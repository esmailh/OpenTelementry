using System.Diagnostics;

namespace SampleOpenTelementry
{
    public static class TelementryConstants
    {
        public const string AppSource = "SampleOpenTelementry";
        public const string AppSourceVersion = "1.0.0";
        public const string MeterName = "SampleOpenTelementry";
        public static readonly ActivitySource RegisteredActivity = new ActivitySource("SampleOpenTelementry.ManualInstrumentations.*");
    }
}
