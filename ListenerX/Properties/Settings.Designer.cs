﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ListenerX.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ChromaSDKEnable {
            get {
                return ((bool)(this["ChromaSDKEnable"]));
            }
            set {
                this["ChromaSDKEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color Background_Playing {
            get {
                return ((global::System.Drawing.Color)(this["Background_Playing"]));
            }
            set {
                this["Background_Playing"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color Background_Pause {
            get {
                return ((global::System.Drawing.Color)(this["Background_Pause"]));
            }
            set {
                this["Background_Pause"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color Position_Foreground {
            get {
                return ((global::System.Drawing.Color)(this["Position_Foreground"]));
            }
            set {
                this["Position_Foreground"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color Position_Background {
            get {
                return ((global::System.Drawing.Color)(this["Position_Background"]));
            }
            set {
                this["Position_Background"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("255, 40, 0")]
        public global::System.Drawing.Color Volume {
            get {
                return ((global::System.Drawing.Color)(this["Volume"]));
            }
            set {
                this["Volume"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ReverseLEDRender {
            get {
                return ((bool)(this["ReverseLEDRender"]));
            }
            set {
                this["ReverseLEDRender"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AlbumCoverRenderEnable {
            get {
                return ((bool)(this["AlbumCoverRenderEnable"]));
            }
            set {
                this["AlbumCoverRenderEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int RefreshRate {
            get {
                return ((int)(this["RefreshRate"]));
            }
            set {
                this["RefreshRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AdaptiveDensity {
            get {
                return ((bool)(this["AdaptiveDensity"]));
            }
            set {
                this["AdaptiveDensity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int Density {
            get {
                return ((int)(this["Density"]));
            }
            set {
                this["Density"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RenderPeakVolumeEnable {
            get {
                return ((bool)(this["RenderPeakVolumeEnable"]));
            }
            set {
                this["RenderPeakVolumeEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("60")]
        public int RenderFPS {
            get {
                return ((int)(this["RenderFPS"]));
            }
            set {
                this["RenderFPS"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool NetworkEnable {
            get {
                return ((bool)(this["NetworkEnable"]));
            }
            set {
                this["NetworkEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SymmetricRenderEnable {
            get {
                return ((bool)(this["SymmetricRenderEnable"]));
            }
            set {
                this["SymmetricRenderEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public byte RenderStyleIndex {
            get {
                return ((byte)(this["RenderStyleIndex"]));
            }
            set {
                this["RenderStyleIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("d2e83cf6ff54792771cc94ea496e2fac")]
        public string LastFMApiKey {
            get {
                return ((string)(this["LastFMApiKey"]));
            }
            set {
                this["LastFMApiKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7bdd7b2585682fe3e12fd8ae7b88d688")]
        public string LastFMApiSecret {
            get {
                return ((string)(this["LastFMApiSecret"]));
            }
            set {
                this["LastFMApiSecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public byte AlbumColorMode {
            get {
                return ((byte)(this["AlbumColorMode"]));
            }
            set {
                this["AlbumColorMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool PeakChroma {
            get {
                return ((bool)(this["PeakChroma"]));
            }
            set {
                this["PeakChroma"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string RefreshToken {
            get {
                return ((string)(this["RefreshToken"]));
            }
            set {
                this["RefreshToken"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AccessToken {
            get {
                return ((string)(this["AccessToken"]));
            }
            set {
                this["AccessToken"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int BlurRadial {
            get {
                return ((int)(this["BlurRadial"]));
            }
            set {
                this["BlurRadial"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int VolumeScale {
            get {
                return ((int)(this["VolumeScale"]));
            }
            set {
                this["VolumeScale"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DisableTrackBackgroundOnMinimized {
            get {
                return ((bool)(this["DisableTrackBackgroundOnMinimized"]));
            }
            set {
                this["DisableTrackBackgroundOnMinimized"] = value;
            }
        }
    }
}