// Generated by ProtoGen, Version=2.4.1.555, Culture=neutral, PublicKeyToken=17b3b1f090c3ea48.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace Gazillion {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class AuthMessages {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    internal static pbd::MessageDescriptor internal__static_Gazillion_AuthTicket__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::Gazillion.AuthTicket, global::Gazillion.AuthTicket.Builder> internal__static_Gazillion_AuthTicket__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static AuthMessages() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJBdXRoTWVzc2FnZXMucHJvdG8SCUdhemlsbGlvbiLiAgoKQXV0aFRpY2tl", 
            "dBIeCgpzZXNzaW9uS2V5GAEgASgMUgpzZXNzaW9uS2V5EiIKDHNlc3Npb25U", 
            "b2tlbhgCIAEoDFIMc2Vzc2lvblRva2VuEhwKCXNlc3Npb25JZBgDIAIoBFIJ", 
            "c2Vzc2lvbklkEiIKDGVycm9yTWVzc2FnZRgEIAEoCVIMZXJyb3JNZXNzYWdl", 
            "EiYKDmZyb250ZW5kU2VydmVyGAUgASgJUg5mcm9udGVuZFNlcnZlchIiCgxm", 
            "cm9udGVuZFBvcnQYBiABKAlSDGZyb250ZW5kUG9ydBImCg5wbGF0Zm9ybVRp", 
            "Y2tldBgHIAEoCVIOcGxhdGZvcm1UaWNrZXQSKAoPcHJlc2FsZVB1cmNoYXNl", 
            "GAggASgIUg9wcmVzYWxlUHVyY2hhc2USFgoGdG9zdXJsGAkgASgJUgZ0b3N1", 
          "cmwSGAoHc3VjY2VzcxgKIAEoCFIHc3VjY2Vzcw=="));
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_Gazillion_AuthTicket__Descriptor = Descriptor.MessageTypes[0];
        internal__static_Gazillion_AuthTicket__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Gazillion.AuthTicket, global::Gazillion.AuthTicket.Builder>(internal__static_Gazillion_AuthTicket__Descriptor,
                new string[] { "SessionKey", "SessionToken", "SessionId", "ErrorMessage", "FrontendServer", "FrontendPort", "PlatformTicket", "PresalePurchase", "Tosurl", "Success", });
        pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
        RegisterAllExtensions(registry);
        return registry;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          }, assigner);
    }
    #endregion
    
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class AuthTicket : pb::GeneratedMessage<AuthTicket, AuthTicket.Builder> {
    private AuthTicket() { }
    private static readonly AuthTicket defaultInstance = new AuthTicket().MakeReadOnly();
    private static readonly string[] _authTicketFieldNames = new string[] { "errorMessage", "frontendPort", "frontendServer", "platformTicket", "presalePurchase", "sessionId", "sessionKey", "sessionToken", "success", "tosurl" };
    private static readonly uint[] _authTicketFieldTags = new uint[] { 34, 50, 42, 58, 64, 24, 10, 18, 80, 74 };
    public static AuthTicket DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override AuthTicket DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override AuthTicket ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::Gazillion.AuthMessages.internal__static_Gazillion_AuthTicket__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<AuthTicket, AuthTicket.Builder> InternalFieldAccessors {
      get { return global::Gazillion.AuthMessages.internal__static_Gazillion_AuthTicket__FieldAccessorTable; }
    }
    
    public const int SessionKeyFieldNumber = 1;
    private bool hasSessionKey;
    private pb::ByteString sessionKey_ = pb::ByteString.Empty;
    public bool HasSessionKey {
      get { return hasSessionKey; }
    }
    public pb::ByteString SessionKey {
      get { return sessionKey_; }
    }
    
    public const int SessionTokenFieldNumber = 2;
    private bool hasSessionToken;
    private pb::ByteString sessionToken_ = pb::ByteString.Empty;
    public bool HasSessionToken {
      get { return hasSessionToken; }
    }
    public pb::ByteString SessionToken {
      get { return sessionToken_; }
    }
    
    public const int SessionIdFieldNumber = 3;
    private bool hasSessionId;
    private ulong sessionId_;
    public bool HasSessionId {
      get { return hasSessionId; }
    }
    public ulong SessionId {
      get { return sessionId_; }
    }
    
    public const int ErrorMessageFieldNumber = 4;
    private bool hasErrorMessage;
    private string errorMessage_ = "";
    public bool HasErrorMessage {
      get { return hasErrorMessage; }
    }
    public string ErrorMessage {
      get { return errorMessage_; }
    }
    
    public const int FrontendServerFieldNumber = 5;
    private bool hasFrontendServer;
    private string frontendServer_ = "";
    public bool HasFrontendServer {
      get { return hasFrontendServer; }
    }
    public string FrontendServer {
      get { return frontendServer_; }
    }
    
    public const int FrontendPortFieldNumber = 6;
    private bool hasFrontendPort;
    private string frontendPort_ = "";
    public bool HasFrontendPort {
      get { return hasFrontendPort; }
    }
    public string FrontendPort {
      get { return frontendPort_; }
    }
    
    public const int PlatformTicketFieldNumber = 7;
    private bool hasPlatformTicket;
    private string platformTicket_ = "";
    public bool HasPlatformTicket {
      get { return hasPlatformTicket; }
    }
    public string PlatformTicket {
      get { return platformTicket_; }
    }
    
    public const int PresalePurchaseFieldNumber = 8;
    private bool hasPresalePurchase;
    private bool presalePurchase_;
    public bool HasPresalePurchase {
      get { return hasPresalePurchase; }
    }
    public bool PresalePurchase {
      get { return presalePurchase_; }
    }
    
    public const int TosurlFieldNumber = 9;
    private bool hasTosurl;
    private string tosurl_ = "";
    public bool HasTosurl {
      get { return hasTosurl; }
    }
    public string Tosurl {
      get { return tosurl_; }
    }
    
    public const int SuccessFieldNumber = 10;
    private bool hasSuccess;
    private bool success_;
    public bool HasSuccess {
      get { return hasSuccess; }
    }
    public bool Success {
      get { return success_; }
    }
    
    public override bool IsInitialized {
      get {
        if (!hasSessionId) return false;
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      CalcSerializedSize();
      string[] field_names = _authTicketFieldNames;
      if (hasSessionKey) {
        output.WriteBytes(1, field_names[6], SessionKey);
      }
      if (hasSessionToken) {
        output.WriteBytes(2, field_names[7], SessionToken);
      }
      if (hasSessionId) {
        output.WriteUInt64(3, field_names[5], SessionId);
      }
      if (hasErrorMessage) {
        output.WriteString(4, field_names[0], ErrorMessage);
      }
      if (hasFrontendServer) {
        output.WriteString(5, field_names[2], FrontendServer);
      }
      if (hasFrontendPort) {
        output.WriteString(6, field_names[1], FrontendPort);
      }
      if (hasPlatformTicket) {
        output.WriteString(7, field_names[3], PlatformTicket);
      }
      if (hasPresalePurchase) {
        output.WriteBool(8, field_names[4], PresalePurchase);
      }
      if (hasTosurl) {
        output.WriteString(9, field_names[9], Tosurl);
      }
      if (hasSuccess) {
        output.WriteBool(10, field_names[8], Success);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        return CalcSerializedSize();
      }
    }
    
    private int CalcSerializedSize() {
      int size = memoizedSerializedSize;
      if (size != -1) return size;
      
      size = 0;
      if (hasSessionKey) {
        size += pb::CodedOutputStream.ComputeBytesSize(1, SessionKey);
      }
      if (hasSessionToken) {
        size += pb::CodedOutputStream.ComputeBytesSize(2, SessionToken);
      }
      if (hasSessionId) {
        size += pb::CodedOutputStream.ComputeUInt64Size(3, SessionId);
      }
      if (hasErrorMessage) {
        size += pb::CodedOutputStream.ComputeStringSize(4, ErrorMessage);
      }
      if (hasFrontendServer) {
        size += pb::CodedOutputStream.ComputeStringSize(5, FrontendServer);
      }
      if (hasFrontendPort) {
        size += pb::CodedOutputStream.ComputeStringSize(6, FrontendPort);
      }
      if (hasPlatformTicket) {
        size += pb::CodedOutputStream.ComputeStringSize(7, PlatformTicket);
      }
      if (hasPresalePurchase) {
        size += pb::CodedOutputStream.ComputeBoolSize(8, PresalePurchase);
      }
      if (hasTosurl) {
        size += pb::CodedOutputStream.ComputeStringSize(9, Tosurl);
      }
      if (hasSuccess) {
        size += pb::CodedOutputStream.ComputeBoolSize(10, Success);
      }
      size += UnknownFields.SerializedSize;
      memoizedSerializedSize = size;
      return size;
    }
    public static AuthTicket ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static AuthTicket ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static AuthTicket ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static AuthTicket ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static AuthTicket ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static AuthTicket ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static AuthTicket ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static AuthTicket ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static AuthTicket ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static AuthTicket ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private AuthTicket MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(AuthTicket prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Builder : pb::GeneratedBuilder<AuthTicket, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(AuthTicket cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private AuthTicket result;
      
      private AuthTicket PrepareBuilder() {
        if (resultIsReadOnly) {
          AuthTicket original = result;
          result = new AuthTicket();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override AuthTicket MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::Gazillion.AuthTicket.Descriptor; }
      }
      
      public override AuthTicket DefaultInstanceForType {
        get { return global::Gazillion.AuthTicket.DefaultInstance; }
      }
      
      public override AuthTicket BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is AuthTicket) {
          return MergeFrom((AuthTicket) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(AuthTicket other) {
        if (other == global::Gazillion.AuthTicket.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasSessionKey) {
          SessionKey = other.SessionKey;
        }
        if (other.HasSessionToken) {
          SessionToken = other.SessionToken;
        }
        if (other.HasSessionId) {
          SessionId = other.SessionId;
        }
        if (other.HasErrorMessage) {
          ErrorMessage = other.ErrorMessage;
        }
        if (other.HasFrontendServer) {
          FrontendServer = other.FrontendServer;
        }
        if (other.HasFrontendPort) {
          FrontendPort = other.FrontendPort;
        }
        if (other.HasPlatformTicket) {
          PlatformTicket = other.PlatformTicket;
        }
        if (other.HasPresalePurchase) {
          PresalePurchase = other.PresalePurchase;
        }
        if (other.HasTosurl) {
          Tosurl = other.Tosurl;
        }
        if (other.HasSuccess) {
          Success = other.Success;
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_authTicketFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _authTicketFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 10: {
              result.hasSessionKey = input.ReadBytes(ref result.sessionKey_);
              break;
            }
            case 18: {
              result.hasSessionToken = input.ReadBytes(ref result.sessionToken_);
              break;
            }
            case 24: {
              result.hasSessionId = input.ReadUInt64(ref result.sessionId_);
              break;
            }
            case 34: {
              result.hasErrorMessage = input.ReadString(ref result.errorMessage_);
              break;
            }
            case 42: {
              result.hasFrontendServer = input.ReadString(ref result.frontendServer_);
              break;
            }
            case 50: {
              result.hasFrontendPort = input.ReadString(ref result.frontendPort_);
              break;
            }
            case 58: {
              result.hasPlatformTicket = input.ReadString(ref result.platformTicket_);
              break;
            }
            case 64: {
              result.hasPresalePurchase = input.ReadBool(ref result.presalePurchase_);
              break;
            }
            case 74: {
              result.hasTosurl = input.ReadString(ref result.tosurl_);
              break;
            }
            case 80: {
              result.hasSuccess = input.ReadBool(ref result.success_);
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasSessionKey {
        get { return result.hasSessionKey; }
      }
      public pb::ByteString SessionKey {
        get { return result.SessionKey; }
        set { SetSessionKey(value); }
      }
      public Builder SetSessionKey(pb::ByteString value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasSessionKey = true;
        result.sessionKey_ = value;
        return this;
      }
      public Builder ClearSessionKey() {
        PrepareBuilder();
        result.hasSessionKey = false;
        result.sessionKey_ = pb::ByteString.Empty;
        return this;
      }
      
      public bool HasSessionToken {
        get { return result.hasSessionToken; }
      }
      public pb::ByteString SessionToken {
        get { return result.SessionToken; }
        set { SetSessionToken(value); }
      }
      public Builder SetSessionToken(pb::ByteString value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasSessionToken = true;
        result.sessionToken_ = value;
        return this;
      }
      public Builder ClearSessionToken() {
        PrepareBuilder();
        result.hasSessionToken = false;
        result.sessionToken_ = pb::ByteString.Empty;
        return this;
      }
      
      public bool HasSessionId {
        get { return result.hasSessionId; }
      }
      public ulong SessionId {
        get { return result.SessionId; }
        set { SetSessionId(value); }
      }
      public Builder SetSessionId(ulong value) {
        PrepareBuilder();
        result.hasSessionId = true;
        result.sessionId_ = value;
        return this;
      }
      public Builder ClearSessionId() {
        PrepareBuilder();
        result.hasSessionId = false;
        result.sessionId_ = 0UL;
        return this;
      }
      
      public bool HasErrorMessage {
        get { return result.hasErrorMessage; }
      }
      public string ErrorMessage {
        get { return result.ErrorMessage; }
        set { SetErrorMessage(value); }
      }
      public Builder SetErrorMessage(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasErrorMessage = true;
        result.errorMessage_ = value;
        return this;
      }
      public Builder ClearErrorMessage() {
        PrepareBuilder();
        result.hasErrorMessage = false;
        result.errorMessage_ = "";
        return this;
      }
      
      public bool HasFrontendServer {
        get { return result.hasFrontendServer; }
      }
      public string FrontendServer {
        get { return result.FrontendServer; }
        set { SetFrontendServer(value); }
      }
      public Builder SetFrontendServer(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasFrontendServer = true;
        result.frontendServer_ = value;
        return this;
      }
      public Builder ClearFrontendServer() {
        PrepareBuilder();
        result.hasFrontendServer = false;
        result.frontendServer_ = "";
        return this;
      }
      
      public bool HasFrontendPort {
        get { return result.hasFrontendPort; }
      }
      public string FrontendPort {
        get { return result.FrontendPort; }
        set { SetFrontendPort(value); }
      }
      public Builder SetFrontendPort(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasFrontendPort = true;
        result.frontendPort_ = value;
        return this;
      }
      public Builder ClearFrontendPort() {
        PrepareBuilder();
        result.hasFrontendPort = false;
        result.frontendPort_ = "";
        return this;
      }
      
      public bool HasPlatformTicket {
        get { return result.hasPlatformTicket; }
      }
      public string PlatformTicket {
        get { return result.PlatformTicket; }
        set { SetPlatformTicket(value); }
      }
      public Builder SetPlatformTicket(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasPlatformTicket = true;
        result.platformTicket_ = value;
        return this;
      }
      public Builder ClearPlatformTicket() {
        PrepareBuilder();
        result.hasPlatformTicket = false;
        result.platformTicket_ = "";
        return this;
      }
      
      public bool HasPresalePurchase {
        get { return result.hasPresalePurchase; }
      }
      public bool PresalePurchase {
        get { return result.PresalePurchase; }
        set { SetPresalePurchase(value); }
      }
      public Builder SetPresalePurchase(bool value) {
        PrepareBuilder();
        result.hasPresalePurchase = true;
        result.presalePurchase_ = value;
        return this;
      }
      public Builder ClearPresalePurchase() {
        PrepareBuilder();
        result.hasPresalePurchase = false;
        result.presalePurchase_ = false;
        return this;
      }
      
      public bool HasTosurl {
        get { return result.hasTosurl; }
      }
      public string Tosurl {
        get { return result.Tosurl; }
        set { SetTosurl(value); }
      }
      public Builder SetTosurl(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasTosurl = true;
        result.tosurl_ = value;
        return this;
      }
      public Builder ClearTosurl() {
        PrepareBuilder();
        result.hasTosurl = false;
        result.tosurl_ = "";
        return this;
      }
      
      public bool HasSuccess {
        get { return result.hasSuccess; }
      }
      public bool Success {
        get { return result.Success; }
        set { SetSuccess(value); }
      }
      public Builder SetSuccess(bool value) {
        PrepareBuilder();
        result.hasSuccess = true;
        result.success_ = value;
        return this;
      }
      public Builder ClearSuccess() {
        PrepareBuilder();
        result.hasSuccess = false;
        result.success_ = false;
        return this;
      }
    }
    static AuthTicket() {
      object.ReferenceEquals(global::Gazillion.AuthMessages.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code
