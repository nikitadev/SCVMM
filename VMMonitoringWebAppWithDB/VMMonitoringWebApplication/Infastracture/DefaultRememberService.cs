namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class DefaultRememberService : IRememberService
    {
        private readonly IDictionary<string, string> persistentDictionary;

        public DefaultRememberService()
        {
            this.persistentDictionary = new Dictionary<string, string>();
        }

        public void Add<T>(string name, T model)
        {
            if (!persistentDictionary.Any(k => k.Key.Equals(name)))
            {
                persistentDictionary.Add(name, model.ToJSON());
            }
            else
            {
                persistentDictionary[name] = model.ToJSON();
            }            
        }

        public T Get<T>(string name)
        {
            string jsonModel;
            if (persistentDictionary.TryGetValue(name, out jsonModel))
            {
                return jsonModel.FromJSON<T>();
            }

            return default(T);
        }

        public bool Check(string name)
        {
            string jsonModel;
            return persistentDictionary.TryGetValue(name, out jsonModel);
        }
    }
}