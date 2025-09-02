namespace Shared.Commons.Options.Polly
{
    public static class RetryPolicyExtensions
    {
        public static TimeSpan CalculateDelay(this RetryPolicyOptions source, int retryAttempt)
        {
            return source.DelayStrategy switch
            {
                // Linear
                _ => TimeSpan.FromMilliseconds(source.RetryDelayInMilliseconds * retryAttempt),
            };
        }
    }
}
