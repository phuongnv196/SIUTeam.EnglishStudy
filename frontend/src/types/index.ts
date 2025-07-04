export * from './auth'
export * from './course'
export * from './student'

// Common types
export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}

export interface PaginatedResponse<T = any> {
  data: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export interface SelectOption {
  label: string
  value: string | number
}

export type LoadingState = 'idle' | 'loading' | 'succeeded' | 'failed'
