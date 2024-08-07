﻿using Gallop;
using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmamusumeResponseAnalyzer.Communications
{
    public class BaseSubscription<T>(string webSocketKey) : ICommand
    {
        public CommandType CommandType { get => CommandType.Subscribe; }
        public string WebSocketKey { get; init; } = webSocketKey;

        private async void Handler(object? _, T e)
        {
            if (!await Server.Send(WebSocketKey, e!))
                BaseSubscriptionHandler -= Handler;
        }
        public WSResponse? Execute()
        {
            BaseSubscriptionHandler += Handler;
            SubscribedClients.Add(WebSocketKey, this);
            return new WSResponse() { Result = WSResponse.WSResponseResultCode.Success };
        }
        public WSResponse? Cancel()
        {
            var response = new WSResponse() { Result = WSResponse.WSResponseResultCode.Success };
            if (SubscribedClients.TryGetValue(WebSocketKey, out var value))
            {
                BaseSubscriptionHandler -= value.Handler;
                SubscribedClients.Remove(WebSocketKey);
            }
            else
            {
                response.Result = WSResponse.WSResponseResultCode.Fail;
                response.Reason = $"未找到SecKey为{WebSocketKey}的订阅";
            }
            return response;
        }

        public static readonly Dictionary<string, BaseSubscription<T>> SubscribedClients = [];
        public static event EventHandler<T> BaseSubscriptionHandler;
        //public static int Signal(T ev)
        public static int Signal(object ev)
        {
            if (BaseSubscriptionHandler != null)
            {
                // hack: 无视ev的具体类型，在这里cast成string
                var serialized = JsonConvert.SerializeObject(ev, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var Handlers = BaseSubscriptionHandler.GetInvocationList().Cast<EventHandler<string>>();
                if (Handlers != null)
                {
                    foreach (var del in Handlers)
                        del.Invoke(null, serialized);
                    return Handlers.ToList().Count;
                }
            }
            // or else goes here.
            return 0;
        }
    }
}