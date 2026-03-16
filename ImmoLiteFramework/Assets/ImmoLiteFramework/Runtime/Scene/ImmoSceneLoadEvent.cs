
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    public sealed class ImmoSceneLoadSuccessEvent : ImmoEvent
    {
        public float LoadDuration { get; }

        public ImmoSceneLoadSuccessEvent(object source, float loadDuration) : base(source)
        {
            LoadDuration = loadDuration;
        }
    }


    public sealed class ImmoSceneLoadProgressEvent : ImmoEvent
    {
        public string SceneName { get; }
        public float Progress { get; }
        public float TotalProgress { get; }

        public ImmoSceneLoadProgressEvent(object source, string sceneName, float progress, float totalProgress) : base(source)
        {
            SceneName = sceneName;
            Progress = progress;
            TotalProgress = totalProgress;
        }
    }


    public sealed class ImmoSceneLoadFailureEvent : ImmoEvent
    {
        public string SceneName { get; }
        public string ErrorMessage { get; }

        public ImmoSceneLoadFailureEvent(object source, string sceneName, string errorMessage) : base(source)
        {
            SceneName = sceneName;
            ErrorMessage = errorMessage;
        }
    }
}