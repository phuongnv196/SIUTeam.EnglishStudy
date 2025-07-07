import type { User, LoginRequest, LoginResponse, RegisterRequest } from '../types/auth'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

// Auth Service
export class AuthService {
  private static TOKEN_KEY = 'token'
  private static REFRESH_TOKEN_KEY = 'refresh_token'
  private static USER_KEY = 'user_data'

  // Get stored token
  static getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY)
  }

  // Get stored refresh token
  static getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY)
  }

  // Get stored user
  static getUser(): User | null {
    const userData = localStorage.getItem(this.USER_KEY)
    return userData ? JSON.parse(userData) : null
  }

  // Store auth data
  static setAuthData(response: LoginResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.token)
    // Note: Backend doesn't return refreshToken yet, using token for now
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.token) 
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user))
  }

  // Clear auth data
  static clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY)
    localStorage.removeItem(this.REFRESH_TOKEN_KEY)
    localStorage.removeItem(this.USER_KEY)
  }

  // Check if user is authenticated
  static isAuthenticated(): boolean {
    const token = this.getToken()
    const user = this.getUser()
    return !!(token && user)
  }

  // Check user role
  static hasRole(requiredRole: string | string[]): boolean {
    const user = this.getUser()
    if (!user) return false

    if (Array.isArray(requiredRole)) {
      return requiredRole.includes(user.role)
    }
    return user.role === requiredRole
  }

  // Login
  static async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Đăng nhập thất bại')
      }

      const data: LoginResponse = await response.json()
      this.setAuthData(data)
      return data
    } catch (error) {
      console.error('Login error:', error)
      throw error
    }
  }

  // Register
  static async register(userData: RegisterRequest): Promise<User> {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Đăng ký thất bại')
      }

      const user: User = await response.json()
      return user
    } catch (error) {
      console.error('Register error:', error)
      throw error
    }
  }

  // Logout
  static async logout(): Promise<void> {
    try {
      const token = this.getToken()
      if (token) {
        await fetch(`${API_BASE_URL}/auth/logout`, {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        })
      }
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      this.clearAuthData()
    }
  }

  // Refresh token
  static async refreshToken(): Promise<string | null> {
    try {
      const refreshToken = this.getRefreshToken()
      if (!refreshToken) return null

      const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken }),
      })

      if (!response.ok) {
        // Backend returns 501 Not Implemented for now
        if (response.status === 501) {
          console.warn('Refresh token not implemented yet')
          return refreshToken // Return current token for now
        }
        this.clearAuthData()
        return null
      }

      const data = await response.json()
      localStorage.setItem(this.TOKEN_KEY, data.token)
      return data.token
    } catch (error) {
      console.error('Refresh token error:', error)
      // For now, don't clear auth data since refresh is not implemented
      return this.getToken()
    }
  }

  // Get current user profile
  static async getCurrentUser(): Promise<User | null> {
    try {
      const token = this.getToken()
      if (!token) return null

      const response = await fetch(`${API_BASE_URL}/users/me`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      })

      if (!response.ok) {
        if (response.status === 401) {
          const newToken = await this.refreshToken()
          if (newToken) {
            return this.getCurrentUser()
          }
        }
        return null
      }

      const user: User = await response.json()
      localStorage.setItem(this.USER_KEY, JSON.stringify(user))
      return user
    } catch (error) {
      console.error('Get current user error:', error)
      return null
    }
  }

  // Update user profile
  static async updateProfile(userData: Partial<User>): Promise<User> {
    try {
      const token = this.getToken()
      const user = this.getUser()
      if (!token || !user) throw new Error('Không có quyền truy cập')

      const response = await fetch(`${API_BASE_URL}/users/${user.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Cập nhật thất bại')
      }

      const updatedUser: User = await response.json()
      localStorage.setItem(this.USER_KEY, JSON.stringify(updatedUser))
      return updatedUser
    } catch (error) {
      console.error('Update profile error:', error)
      throw error
    }
  }

  // Change password
  static async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    try {
      const token = this.getToken()
      if (!token) throw new Error('Không có quyền truy cập')

      const response = await fetch(`${API_BASE_URL}/auth/change-password`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ currentPassword, newPassword }),
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Đổi mật khẩu thất bại')
      }
    } catch (error) {
      console.error('Change password error:', error)
      throw error
    }
  }
}

export default AuthService
