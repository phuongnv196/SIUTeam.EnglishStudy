export interface Course {
  id: string
  title: string
  description: string
  level: 'Beginner' | 'Intermediate' | 'Advanced'
  duration: string
  students: number
  instructor: string
  progress?: number
  createdAt: string
  updatedAt: string
}

export interface CourseCreateRequest {
  title: string
  description: string
  level: 'Beginner' | 'Intermediate' | 'Advanced'
  duration: string
  instructor: string
}

export interface CourseUpdateRequest extends Partial<CourseCreateRequest> {
  id: string
}
