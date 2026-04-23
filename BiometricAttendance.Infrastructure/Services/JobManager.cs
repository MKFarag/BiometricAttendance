using Hangfire;

namespace BiometricAttendance.Infrastructure.Services;

public class JobManager : IJobManager
{
    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    public string Enqueue(Expression<Action> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    public string Enqueue(Expression<Func<Task>> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    public string Enqueue<T>(Expression<Action<T>> methodCall)
        => BackgroundJob.Enqueue<T>(methodCall);

    /// <summary>
    /// Creates a new fire-and-forget job based on a given method call expression.
    /// </summary>
    /// <param name="methodCall">Method call expression that will be marshalled to a server.</param>
    /// <returns>Unique identifier of a background job.</returns>
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        => BackgroundJob.Enqueue<T>(methodCall);

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <returns>Unique identifier of the created job.</returns>
    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);

    /// <summary>
    /// Creates a new background job based on a specified method call expression and schedules it to be enqueued after a given delay.
    /// </summary>
    /// <param name="methodCall">Instance method call expression that will be marshalled to the Server.</param>
    /// <param name="delay">Delay, after which the job will be enqueued.</param>
    /// <returns>Unique identifier of the created job.</returns>
    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);
}