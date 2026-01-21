using UnityEngine;
using System;


namespace Immojoy.LiteFramework.Runtime
{
    public enum UiLayer
    {
        None,
        Background,
        Normal,
        Popup,
        Top,
        System
    }


    [AttributeUsage(AttributeTargets.Class)]    
    public class UiLayerAttribute : Attribute
    {
        public UiLayer LayerType;
        public UiLayerAttribute(UiLayer layerType) => LayerType = layerType;
    }
}