using Common.Models;
using Newtonsoft.Json.Linq;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsentRequester
{
    public delegate void ConsentResponseReceivedHandler(object sender, ConsentResponse response);

    public class ConsentReceiver
    {
        public event ConsentResponseReceivedHandler ConsentResponseReceived;

        private Pusher pusher;

        public ConsentReceiver()
        {
            pusher = new Pusher("3d986af38ba72547d258", new PusherOptions()
            {
                Cluster = "eu"
            });
            pusher.ConnectionStateChanged += pusher_ConnectionStateChanged;
            pusher.Error += pusher_Error;
            pusher.Connect();
        }

        public void ListenFor(string bankId)
        {
            try
            {
                var channel = pusher.Subscribe(bankId);

                channel.Bind("Consent", (dynamic msg) =>
                {
                    var j = msg as JObject;
                    var response = j.ToObject<ConsentResponse>();
                    this.ConsentResponseReceived?.Invoke(this, response);
                });
            }
            catch(Exception ex)
            {
                //TODO: Find out why one of the banks is listened for twice.

                // Yes, I sometimes leave things unfinished, but mark them with //TODO:
            }
        }

        private void pusher_ConnectionStateChanged(object sender, ConnectionState state)
        {
            Console.WriteLine("Connection state: " + state.ToString());
        }

        private void pusher_Error(object sender, PusherException error)
        {
            Console.WriteLine("Pusher Error: " + error.ToString());
        }

    }
}
