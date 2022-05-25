using Nancy;
using Nancy.ModelBinding;

namespace FaceNetCore4
{
   

   public class HelloWorldModule: NancyModule
    {

        public class SimpleRequest
        {
            public string user { get; set; }
            public string id { get; set; }
        }
        public HelloWorldModule()
        {
            Get("/", p => "Hello World");
            Get("/hello/{name}", args =>
            {
                string name = args.name;
                var response = string.Format("Hello {0} ! ", name);
                return response;
            });

            Post("/",  _ =>
            {
                var request = this.Bind<SimpleRequest>();
                return request.user;
            });
        }
    }
}
