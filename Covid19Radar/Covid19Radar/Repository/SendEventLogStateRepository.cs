// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public enum SendEventLogState
    {
        NotSet = 0,
        Disable = -1,
        Enable = 1
    }

    public class EventType
    {
        public string Type { get; }
        public string SubType { get; }

        public EventType(string type, string subType)
        {
            Type = type;
            SubType = subType;
        }

        public override string ToString()
        {
            return $"{Type}-{SubType}";
        }
    }

    public interface ISendEventLogStateRepository
    {
        public static readonly EventType EVENT_TYPE_EXPOSURE_NOTIFIED = new EventType("ExposureNotification", "ExposureNotified");
        public static readonly EventType EVENT_TYPE_EXPOSURE_DATA = new EventType("ExposureNotification", "ExposureData");

        public static readonly EventType[] EVENT_TYPE_ALL = new EventType[] {
            EVENT_TYPE_EXPOSURE_NOTIFIED,
            EVENT_TYPE_EXPOSURE_DATA,
        };

        void SetSendEventLogState(EventType eventType, SendEventLogState state);

        SendEventLogState GetSendEventLogState(EventType eventType);
    }

    public class SendEventLogStateRepository : ISendEventLogStateRepository
    {
        private const string EMPTY_DICT = "{}";

        private readonly IPreferencesService _preferencesService;
        private readonly ILoggerService _loggerService;

        public SendEventLogStateRepository(
            IPreferencesService preferencesService,
            ILoggerService loggerService
            )
        {
            _preferencesService = preferencesService;
            _loggerService = loggerService;
        }

        public SendEventLogState GetSendEventLogState(EventType eventType)
        {
            string stateString = EMPTY_DICT;

            try
            {
                _loggerService.StartMethod();

                if (!_preferencesService.ContainsKey(PreferenceKey.SendEventLogState))
                {
                    return SendEventLogState.NotSet;
                }

                stateString = _preferencesService.GetStringValue(
                    PreferenceKey.SendEventLogState,
                    EMPTY_DICT
                    );

                IDictionary<string, int> stateDict
                    = JsonConvert.DeserializeObject<IDictionary<string, int>>(stateString);

                if (!stateDict.ContainsKey(eventType.ToString()))
                {
                    return SendEventLogState.NotSet;
                }

                int value = stateDict[eventType.ToString()];
                return (SendEventLogState)Enum.ToObject(typeof(SendEventLogState), value);
            }
            catch (JsonReaderException exception)
            {
                _preferencesService.SetStringValue(PreferenceKey.SendEventLogState, EMPTY_DICT);

                _loggerService.Exception($"JsonSerializationException {stateString}", exception);
                _loggerService.Warning($"Preference-key {PreferenceKey.SendEventLogState} has been initialized.");
            }
            finally
            {
                _loggerService.EndMethod();
            }

            return SendEventLogState.NotSet;
        }

        public void SetSendEventLogState(EventType eventType, SendEventLogState state)
        {
            try
            {
                _loggerService.StartMethod();

                string stateString = EMPTY_DICT;
                IDictionary<string, int> stateDict = new Dictionary<string, int>();

                try
                {
                    if (_preferencesService.ContainsKey(PreferenceKey.SendEventLogState))
                    {
                        stateString = _preferencesService.GetStringValue(
                            PreferenceKey.SendEventLogState,
                            EMPTY_DICT
                            );

                        stateDict = JsonConvert.DeserializeObject<IDictionary<string, int>>(stateString);
                    }
                }
                catch (JsonReaderException exception)
                {
                    _loggerService.Exception($"JsonSerializationException {stateString}", exception);
                }

                stateDict[eventType.ToString()] = (int)state;

                string newStateString = JsonConvert.SerializeObject(stateDict);
                _preferencesService.SetStringValue(PreferenceKey.SendEventLogState, newStateString);

            }
            finally
            {
                _loggerService.EndMethod();

            }
        }
    }
}
