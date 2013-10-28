namespace TfsCommunicator.UnitTests
{
    #region

    using Rhino.Mocks;
    using Rhino.Mocks.Interfaces;

    #endregion

    // thank you, http://stackoverflow.com/questions/630037/rhino-mocks-change-behaviour-of-stubs
    internal static class MockExtensions
    {
        public static void ClearBehavior<T>(this T obj)
        {
            // Switch back to record and then to replay - that 
            // clears all behaviour and we can program new behavior.
            // Record/Replay do not occur otherwise in our tests, that another method of
            // using Rhino Mocks.

            obj.BackToRecord(BackToRecordOptions.All);
            obj.Replay();
        }
    }
}
