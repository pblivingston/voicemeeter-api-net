// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Exceptions;
using PBLivingston.VoicemeeterAPI.Utilities;

namespace PBLivingston.VoicemeeterAPI;

partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc/>
    public bool IsParamsDirty()
    {
        LoginGuard();

        var result = _vmrApi.IsParametersDirty();

        return result switch
        {
            Response.Ok => false,
            Response.Dirty => true,
            _ => throw new RemoteException($"ParamsDirty failed - {result}"),
        };
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out float value)
    {
        LoginGuard();

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw new GetParamException<float>(result, param, value, typeof(float));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out int value)
    {
        GetParam(param, out float val);

        value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
            throw new GetParamException<float>(Response.TypeMismatch, param, val, typeof(int));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out bool value)
    {
        GetParam(param, out float val);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
            throw new GetParamException<float>(Response.TypeMismatch, param, val, typeof(bool));

        value = v == 1;
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out string value)
    {
        LoginGuard();

        var result = _vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw new GetParamException<string>(result, param, value, typeof(string));
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

        throw new TypeNotSupportedException<T>(nameof(value), SupportedTypes.ParamTypes);
    }
}
