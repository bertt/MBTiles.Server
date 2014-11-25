using System.Web.Http.Filters;

namespace MBTilesServer.Filters
{
    public class ElmahErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // note this logs everything *except* httpresponseexception
            if (actionExecutedContext.Exception != null) Elmah.ErrorSignal.FromCurrentContext().Raise(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}
