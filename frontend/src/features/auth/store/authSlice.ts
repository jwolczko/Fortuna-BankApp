import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { loadAuthSession } from '../authSession';
import type { StoredAuthSession } from '../authSession';
import type { AuthState } from '../types/auth.types';

const restoredSession = loadAuthSession();

const initialState: AuthState = restoredSession
  ? {
      ...restoredSession,
      isAuthenticated: true,
    }
  : {
      token: null,
      userName: null,
      customerId: null,
      expiresAtUtc: null,
      isAuthenticated: false,
    };

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<StoredAuthSession>,
    ) => {
      state.token = action.payload.token;
      state.userName = action.payload.userName;
      state.customerId = action.payload.customerId;
      state.expiresAtUtc = action.payload.expiresAtUtc;
      state.isAuthenticated = true;
    },
    clearCredentials: (state) => {
      state.token = null;
      state.userName = null;
      state.customerId = null;
      state.expiresAtUtc = null;
      state.isAuthenticated = false;
    },
  },
});

export const { setCredentials, clearCredentials } = authSlice.actions;
export default authSlice.reducer;
