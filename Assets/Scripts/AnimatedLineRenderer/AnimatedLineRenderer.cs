using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(LineRenderer))]
public class AnimatedLineRenderer : MonoBehaviour {
    [Tooltip("The minimum distance that must be in between line segments (0 for infinite). Attempts to make lines with distances smaller than this will fail.")]
    public float MinimumDistance = 0.0f;

    [Tooltip("How much seconds it takes to draw out a unit")]
    public float SecondsPerUnit = 0.1f;

    [Tooltip("Z position of line. Please change both, i couldn't handle it in another way")]
    public int ZPosition = -1;
    public static int ZPositionStatic = -1; // So we can use it for simply showing the line and not animating it

    [Tooltip("Particle system that should be displayed on draw")]
    public GameObject DrawParticles;

    private struct QueueItem {
        public Vector3 Position;
        public float ElapsedSeconds;
        public float TotalSeconds;
        public float TotalSecondsInverse;
    }

    private LineRenderer lineRenderer;
    private readonly Queue<QueueItem> queue = new Queue<QueueItem>();
    private QueueItem prev;
    private QueueItem current;
    private QueueItem? lastQueued;
    private int index = -1;
    private float remainder;
    private GameObject drawParticleObject;
    private ParticleSystem drawParticleSystem;

    private void ProcessCurrent() {
        if (current.ElapsedSeconds == current.TotalSeconds) {
            if (queue.Count == 0) {
                return;
            } else {
                prev = current;
                current = queue.Dequeue();
                if (++index == 0) {
                    lineRenderer.numPositions = 1;
                    StartPoint = current.Position;
                    current.ElapsedSeconds = current.TotalSeconds = current.TotalSecondsInverse = 0.0f;
                    lineRenderer.SetPosition(0, current.Position);
                    return;
                } else {
                    lineRenderer.numPositions = index + 1;
                }
            }
        }

        float newElapsedSeconds = current.ElapsedSeconds + Time.deltaTime + remainder;
        if (newElapsedSeconds > current.TotalSeconds) {
            remainder = newElapsedSeconds - current.TotalSeconds;
            current.ElapsedSeconds = current.TotalSeconds;
        } else {
            remainder = 0.0f;
            current.ElapsedSeconds = newElapsedSeconds;
        }
        current.ElapsedSeconds = Mathf.Min(current.TotalSeconds, current.ElapsedSeconds + Time.deltaTime);
        float lerp = current.TotalSecondsInverse * current.ElapsedSeconds;
        EndPoint = Vector3.Lerp(prev.Position, current.Position, lerp);
        lineRenderer.SetPosition(index, EndPoint);

        if (drawParticleObject != null) {
            drawParticleObject.transform.position = EndPoint;
        }
    }

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();

        if (DrawParticles != null) {
            drawParticleObject = Instantiate(DrawParticles, transform);

            drawParticleSystem = drawParticleObject.GetComponent<ParticleSystem>();
            drawParticleSystem.Stop();
        }
    }

    private void Update() {
        if (drawParticleSystem != null)
            if (queue.Count == 0) {
                drawParticleSystem.Stop();
            } else {
                drawParticleSystem.Play();
            }

        ProcessCurrent();
    }

    private IEnumerator ResetAfterSecondsInternal(float seconds, Action callback) {
        if (seconds <= 0.0f) {
            Reset();
            if (callback != null) {
                callback();
            }
            yield break;
        }

        float elapsedSeconds = 0.0f;
        float secondsInverse = 1.0f / seconds;
        Color c1 = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, 1.0f);
        Color c2 = new Color(lineRenderer.endColor.r, lineRenderer.endColor.g, lineRenderer.endColor.b, 1.0f);

        while (elapsedSeconds < seconds) {
            float a = 1.0f - (secondsInverse * elapsedSeconds);
            elapsedSeconds += Time.deltaTime;
            c1.a = a;
            c2.a = a;
            lineRenderer.startColor = c1;
            lineRenderer.endColor = c2;
            yield return new WaitForSeconds(0.01f);
        }
        Reset();
        if (callback != null) {
            callback();
        }
    }

    /// <summary>
    /// Enqueue a line segment, using SecondsPerLine for the duration
    /// </summary>
    /// <param name="pos">Position of the line segment</param>
    /// <returns>True if enqueued, false if not</returns>
    public bool Enqueue(Vector3 pos) {
        return Enqueue(pos, lastQueued == null ? SecondsPerUnit : SecondsPerUnit * Vector3.Distance(lastQueued.Value.Position, pos));
    }

    /// <summary>
    /// Enqueue a line segment
    /// </summary>
    /// <param name="pos">Position of the line segment</param>
    /// <param name="duration">Duration the line segment should take to become the full length</param>
    /// <returns>True if enqueued, false if not</returns>
    public bool Enqueue(Vector3 pos, float duration) {
        if (Resetting) {
            return false;
        } else if (MinimumDistance > 0.0f && lastQueued != null) {
            Vector3 prevPos = lastQueued.Value.Position;
            float distance = Vector3.Distance(prevPos, pos);
            if (distance < MinimumDistance) {
                return false;
            } else {
                // Debug.LogFormat("Distance between {0} and {1}: {2}", prevPos, pos, distance);
            }
        }

        pos.z = ZPosition;
        float durationSeconds = Mathf.Max(0.0f, duration);
        QueueItem item = new QueueItem {
            Position = pos,
            TotalSecondsInverse = (durationSeconds == 0.0f ? 0.0f : 1.0f / durationSeconds),
            TotalSeconds = durationSeconds,
            ElapsedSeconds = 0.0f
        };
        queue.Enqueue(item);
        lastQueued = item;

        return true;
    }

    /// <summary>
    /// Reset the line renderer, setting everything back to defaults
    /// </summary>
    public void Reset() {
        index = -1;
        prev = current = new QueueItem();
        lastQueued = null;
        if (lineRenderer != null) {
            lineRenderer.numPositions = 0;
        }
        remainder = 0.0f;
        queue.Clear();
        Resetting = false;
        StartPoint = EndPoint = Vector3.zero;

        lineRenderer.numPositions = 0;
    }

    /// <summary>
    /// Reset the line renderer, fading out smoothly over seconds
    /// </summary>
    /// <param name="seconds">Seconds to fade out</param>
    public void ResetAfterSeconds(float seconds) {
        ResetAfterSeconds(seconds, null, null);
    }

    /// <summary>
    /// Reset the line renderer, fading out smoothly over seconds
    /// </summary>
    /// <param name="seconds">Seconds to fade out</param>
    /// <param name="endPoint">Force the end point to a new value (optional)</param>
    public void ResetAfterSeconds(float seconds, Vector3? endPoint) {
        ResetAfterSeconds(seconds, endPoint, null);
    }

    /// <summary>
    /// Reset the line renderer, fading out smoothly over seconds
    /// </summary>
    /// <param name="seconds">Seconds to fade out</param>
    /// <param name="callback">Callback when the fade out is finished</param>
    public void ResetAfterSeconds(float seconds, Vector3? endPoint, Action callback) {
        Resetting = true;
        if (endPoint != null) {
            current.Position = endPoint.Value;
            if (index > 0) {
                lineRenderer.SetPosition(index, endPoint.Value);
            }
        }
        StartCoroutine(ResetAfterSecondsInternal(seconds, callback));
    }

    /// <summary>
    /// The Unity Line Renderer
    /// </summary>
    public LineRenderer LineRenderer { get { return lineRenderer; } }

    /// <summary>
    /// The current line start point (point index 0)
    /// </summary>
    public Vector3 StartPoint { get; private set; }

    /// <summary>
    /// The current line end point (point index n - 1)
    /// </summary>
    public Vector3 EndPoint { get; private set; }

    /// <summary>
    /// Is the line renderer in the process of resetting?
    /// </summary>
    public bool Resetting { get; private set; }
}