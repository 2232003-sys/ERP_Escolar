import { api } from '@/lib/api/client'

export interface User {
  userId: string;
  username: string;
  email: string;
  roles: string[];
  permisos: string[];
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  userId: string;
  username: string;
  email: string;
  roles: string[];
  permisos: string[];
}

export const authService = {
  login: async (username: string, password: string): Promise<LoginResponse> => {
    try {
      const response = await api.post('/Auth/login', {
        username,
        password
      });

      const data: LoginResponse = response.data;
      if (data.accessToken) {
        localStorage.setItem('token', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('user', JSON.stringify({
          userId: data.userId,
          username: data.username,
          email: data.email,
          roles: data.roles,
          permisos: data.permisos
        }));
      }
      return data;
    } catch (error) {
      throw new Error('Credenciales incorrectas');
    }
  },

  logout: (): void => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    window.location.href = '/login';
  },

  getToken: (): string | null => localStorage.getItem('token'),

  getUser: (): User | null => {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },

  isAuthenticated: (): boolean => {
    const token = authService.getToken();
    return !!token;
  }
};