// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

#if NET9_0_OR_GREATER
global using LockObject = System.Threading.Lock;
#else
global using LockObject = System.Object;
#endif
