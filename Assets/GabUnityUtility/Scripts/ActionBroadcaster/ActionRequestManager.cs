using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabUnity.ActionBroadcaster
{
    public class ActionRequestManager: MonoSingleton<ActionRequestManager>
    {
        private readonly Dictionary<System.Type, ActionRequestHandler_Base> handler_list = new();

        public static void RegisterHandler(System.Type handleType, ActionRequestHandler_Base handler)
        {
            Instance.handler_list.Add(handleType, handler);
        }

        public static bool Request(ActionRequest request)
        {
            return Instance.handler_list[request.GetType()].Request(request);
        }
    }
}
