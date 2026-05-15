// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using PBLivingston.VoicemeeterAPI.Types;
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

        On_Query_Start(level);

        var result = _vmrApi.IsParametersDirty();

        switch (result)
        {
            case Response.Ok:
                On_Query_Success(Response.Ok, level);
                return false;

            case Response.Dirty:
                On_ParamsDirty(level);
                return true;

            default:
                throw On_Method_Error(result);
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

        On_GetParam_Start(param);

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw On_GetParam_Error(result, param, value, typeof(float));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out float value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out value);

        On_GetParam_Success(param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out int value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out float val);

        value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
            throw On_GetParam_Error(Response.TypeMismatch, param, val, typeof(int));

        On_GetParam_Success(param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out bool value)
    {
        using var scope = BeginInstanceScope();

        GetParam_Float(param, out float val);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
            throw On_GetParam_Error(Response.TypeMismatch, param, val, typeof(bool));

        value = v == 1;

        On_GetParam_Success(param, value);
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    internal void GetParam_String(string param, out string value)
    {
        LoginGuard();

        On_GetParam_Start(param);

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw On_GetParam_Error(result, param, value, typeof(string));

        On_GetParam_Success(param, value);
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

        throw GeneralDispatch.On_TypeNotSupported(_logger, typeof(T), nameof(value), SupportedTypes.ParamTypes);
    }
}
