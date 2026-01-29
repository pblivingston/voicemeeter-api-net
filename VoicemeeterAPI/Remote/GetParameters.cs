// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

using System;
using VoicemeeterAPI.Types;
using VoicemeeterAPI.Messages;

namespace VoicemeeterAPI;

partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc/>
    public bool ParamsDirty()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

        if (LoginStatus is not LoginResponse.Ok)
            throw new RemoteAccessException(nameof(ParamsDirty), LoginStatus);

        var result = (Response)_vmrApi.IsParametersDirty();

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
        if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

        if (LoginStatus is not LoginResponse.Ok)
            throw new RemoteAccessException(nameof(ParamsDirty), LoginStatus);

        var result = (Response)_vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw new RemoteException($"GetParam failed - {result}; requested param: {param}, returned value: {value}");
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out int value)
    {
        GetParam(param, out float val);
        value = (int)val;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out bool value)
    {
        GetParam(param, out float val);
        value = val != 0;
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out string value)
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(Remote));

        if (LoginStatus is not LoginResponse.Ok)
            throw new RemoteAccessException(nameof(ParamsDirty), LoginStatus);

        var result = (Response)_vmrApi.GetParameter(param, out value);

        if (result != Response.Ok)
            throw new RemoteException($"GetParam failed - {result}; requested param: {param}, returned value: {value}");
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

        throw new NotSupportedException($"'{nameof(value)}' type '{typeof(T)}' is not supported for GetParams.");
    }
}
