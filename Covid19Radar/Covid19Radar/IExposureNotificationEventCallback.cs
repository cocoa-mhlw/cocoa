namespace Covid19Radar
{
    public interface IExposureNotificationEventCallback
    {
        public void OnEnabled() { }
        public void OnDeclined() { }
        public void OnGetTekHistoryAllowed() { }
        public void OnPreauthorizeAllowed() { }
    }
}
