﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace PSEBONLINE.epunjabschool {
    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="service_psebSoap", Namespace="http://tempuri.org/")]
    public partial class service_pseb : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback get9thClassAllStudentsOperationCompleted;
        
        private System.Threading.SendOrPostCallback get9thClassStudentDetails_ByStudentsOperationCompleted;
        
        private System.Threading.SendOrPostCallback get9thClassStudentDetails_ByUIDOperationCompleted;
        
        private System.Threading.SendOrPostCallback getSchoolDetailsByUdiseCodeOperationCompleted;
        
        private System.Threading.SendOrPostCallback getStaffDetailsOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public service_pseb() {
            this.Url = global::PSEBONLINE.Properties.Settings.Default.PSEBONLINE_epunjabschool_service_pseb;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event get9thClassAllStudentsCompletedEventHandler get9thClassAllStudentsCompleted;
        
        /// <remarks/>
        public event get9thClassStudentDetails_ByStudentsCompletedEventHandler get9thClassStudentDetails_ByStudentsCompleted;
        
        /// <remarks/>
        public event get9thClassStudentDetails_ByUIDCompletedEventHandler get9thClassStudentDetails_ByUIDCompleted;
        
        /// <remarks/>
        public event getSchoolDetailsByUdiseCodeCompletedEventHandler getSchoolDetailsByUdiseCodeCompleted;
        
        /// <remarks/>
        public event getStaffDetailsCompletedEventHandler getStaffDetailsCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/get9thClassAllStudents", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string get9thClassAllStudents(string paramerter) {
            object[] results = this.Invoke("get9thClassAllStudents", new object[] {
                        paramerter});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void get9thClassAllStudentsAsync(string paramerter) {
            this.get9thClassAllStudentsAsync(paramerter, null);
        }
        
        /// <remarks/>
        public void get9thClassAllStudentsAsync(string paramerter, object userState) {
            if ((this.get9thClassAllStudentsOperationCompleted == null)) {
                this.get9thClassAllStudentsOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget9thClassAllStudentsOperationCompleted);
            }
            this.InvokeAsync("get9thClassAllStudents", new object[] {
                        paramerter}, this.get9thClassAllStudentsOperationCompleted, userState);
        }
        
        private void Onget9thClassAllStudentsOperationCompleted(object arg) {
            if ((this.get9thClassAllStudentsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get9thClassAllStudentsCompleted(this, new get9thClassAllStudentsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/get9thClassStudentDetails_ByStudents", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string get9thClassStudentDetails_ByStudents(string paramerter) {
            object[] results = this.Invoke("get9thClassStudentDetails_ByStudents", new object[] {
                        paramerter});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void get9thClassStudentDetails_ByStudentsAsync(string paramerter) {
            this.get9thClassStudentDetails_ByStudentsAsync(paramerter, null);
        }
        
        /// <remarks/>
        public void get9thClassStudentDetails_ByStudentsAsync(string paramerter, object userState) {
            if ((this.get9thClassStudentDetails_ByStudentsOperationCompleted == null)) {
                this.get9thClassStudentDetails_ByStudentsOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget9thClassStudentDetails_ByStudentsOperationCompleted);
            }
            this.InvokeAsync("get9thClassStudentDetails_ByStudents", new object[] {
                        paramerter}, this.get9thClassStudentDetails_ByStudentsOperationCompleted, userState);
        }
        
        private void Onget9thClassStudentDetails_ByStudentsOperationCompleted(object arg) {
            if ((this.get9thClassStudentDetails_ByStudentsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get9thClassStudentDetails_ByStudentsCompleted(this, new get9thClassStudentDetails_ByStudentsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/get9thClassStudentDetails_ByUID", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string get9thClassStudentDetails_ByUID(string paramerter) {
            object[] results = this.Invoke("get9thClassStudentDetails_ByUID", new object[] {
                        paramerter});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void get9thClassStudentDetails_ByUIDAsync(string paramerter) {
            this.get9thClassStudentDetails_ByUIDAsync(paramerter, null);
        }
        
        /// <remarks/>
        public void get9thClassStudentDetails_ByUIDAsync(string paramerter, object userState) {
            if ((this.get9thClassStudentDetails_ByUIDOperationCompleted == null)) {
                this.get9thClassStudentDetails_ByUIDOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget9thClassStudentDetails_ByUIDOperationCompleted);
            }
            this.InvokeAsync("get9thClassStudentDetails_ByUID", new object[] {
                        paramerter}, this.get9thClassStudentDetails_ByUIDOperationCompleted, userState);
        }
        
        private void Onget9thClassStudentDetails_ByUIDOperationCompleted(object arg) {
            if ((this.get9thClassStudentDetails_ByUIDCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get9thClassStudentDetails_ByUIDCompleted(this, new get9thClassStudentDetails_ByUIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/getSchoolDetailsByUdiseCode", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getSchoolDetailsByUdiseCode(string paramerter) {
            object[] results = this.Invoke("getSchoolDetailsByUdiseCode", new object[] {
                        paramerter});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getSchoolDetailsByUdiseCodeAsync(string paramerter) {
            this.getSchoolDetailsByUdiseCodeAsync(paramerter, null);
        }
        
        /// <remarks/>
        public void getSchoolDetailsByUdiseCodeAsync(string paramerter, object userState) {
            if ((this.getSchoolDetailsByUdiseCodeOperationCompleted == null)) {
                this.getSchoolDetailsByUdiseCodeOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetSchoolDetailsByUdiseCodeOperationCompleted);
            }
            this.InvokeAsync("getSchoolDetailsByUdiseCode", new object[] {
                        paramerter}, this.getSchoolDetailsByUdiseCodeOperationCompleted, userState);
        }
        
        private void OngetSchoolDetailsByUdiseCodeOperationCompleted(object arg) {
            if ((this.getSchoolDetailsByUdiseCodeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getSchoolDetailsByUdiseCodeCompleted(this, new getSchoolDetailsByUdiseCodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/getStaffDetails", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getStaffDetails(string paramerter) {
            object[] results = this.Invoke("getStaffDetails", new object[] {
                        paramerter});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getStaffDetailsAsync(string paramerter) {
            this.getStaffDetailsAsync(paramerter, null);
        }
        
        /// <remarks/>
        public void getStaffDetailsAsync(string paramerter, object userState) {
            if ((this.getStaffDetailsOperationCompleted == null)) {
                this.getStaffDetailsOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStaffDetailsOperationCompleted);
            }
            this.InvokeAsync("getStaffDetails", new object[] {
                        paramerter}, this.getStaffDetailsOperationCompleted, userState);
        }
        
        private void OngetStaffDetailsOperationCompleted(object arg) {
            if ((this.getStaffDetailsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getStaffDetailsCompleted(this, new getStaffDetailsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    public delegate void get9thClassAllStudentsCompletedEventHandler(object sender, get9thClassAllStudentsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get9thClassAllStudentsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get9thClassAllStudentsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    public delegate void get9thClassStudentDetails_ByStudentsCompletedEventHandler(object sender, get9thClassStudentDetails_ByStudentsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get9thClassStudentDetails_ByStudentsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get9thClassStudentDetails_ByStudentsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    public delegate void get9thClassStudentDetails_ByUIDCompletedEventHandler(object sender, get9thClassStudentDetails_ByUIDCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get9thClassStudentDetails_ByUIDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get9thClassStudentDetails_ByUIDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    public delegate void getSchoolDetailsByUdiseCodeCompletedEventHandler(object sender, getSchoolDetailsByUdiseCodeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getSchoolDetailsByUdiseCodeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getSchoolDetailsByUdiseCodeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    public delegate void getStaffDetailsCompletedEventHandler(object sender, getStaffDetailsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getStaffDetailsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getStaffDetailsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591