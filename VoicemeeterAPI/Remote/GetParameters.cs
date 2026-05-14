// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.EventManagement;
using PBLivingston.VoicemeeterAPI.Utilities;
using Microsoft.Extensions.Logging;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc/>
    internal bool Query_ParamsDirty()
    {
        LoginGuard();

        var level = LogLevel.Trace;

        RemoteDispatch.Query_Start(_logger, level);

        var result = _vmrApi.IsParametersDirty();

        switch (result)
        {
            case Response.Ok:
                RemoteDispatch.Query_Success(_logger, level, Response.Ok);
                return false;

            case Response.Dirty:
                OnParamsDirty(level);
                return true;

            default:
                throw RemoteDispatch.Method_Error(_logger, result);
        }
    }

    /// <inheritdoc/>
    public bool IsParamsDirty()
    {
        using var scope = BeginInstanceScope();

        return Query_ParamsDirty();
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    internal void GetParam_Float(string param, out float value)
    {
        LoginGuard();

        RemoteDispatch.GetParam_Start(_logger, param);

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw RemoteDispatch.GetParam_Error(_logger, result, param, value, typeof(float));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out float value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out value);

        RemoteDispatch.GetParam_Success(_logger, param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out int value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out float val);

        value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
            throw RemoteDispatch.GetParam_Error(_logger, Response.TypeMismatch, param, val, typeof(int));

        RemoteDispatch.GetParam_Success(_logger, param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out bool value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out float val);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
            throw RemoteDispatch.GetParam_Error(_logger, Response.TypeMismatch, param, val, typeof(bool));

        value = v == 1;

        RemoteDispatch.GetParam_Success(_logger, param, value);
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    internal void GetParam_String(string param, out string value)
    {
        LoginGuard();

        RemoteDispatch.GetParam_Start(_logger, param);

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw RemoteDispatch.GetParam_Error(_logger, result, param, value, typeof(string));

        RemoteDispatch.GetParam_Success(_logger, param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out string value)
    {
        using var scope = BeginInstanceScope();

        GetParam_String(param, out value);
    }

    #endregion

    /// <inheritdoc/>
    void IRemote.GetParam<T>(string param, out T value)
    {
        if (typeof(T) == typeof(float))
        {
            GetParam(param, out float val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(int))
        {
            GetParam(param, out int val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(bool))
        {
            GetParam(param, out bool val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(string))
        {
            GetParam(param, out string val);
            value = (T)(object)val;
            return;
        }

        throw GeneralDispatch.TypeNotSupported(_logger, typeof(T), nameof(value), SupportedTypes.ParamTypes);
    }

    private void OnParamsDirty(LogLevel level)
    {
        RemoteDispatch.Dirty_Success(_logger, level, this, ParamsDirty, nameof(IsParamsDirty));
    }
}
