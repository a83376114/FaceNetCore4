using Nancy;
using Nancy.ModelBinding;

namespace FaceNetCore4
{
    public class IdentifyModule : NancyModule
    {
        class FaceRequst
        {
            public string feature { get; }
        }

        public IdentifyModule()
        {
            Post("/", _ =>
            {
                var request = this.Bind<FaceRequst>();
                //string feature = request.feature;
                Detect detect = new Detect();
                //detect.identify_by_feature(feature);
                return "";
            });
        }
    }
}
