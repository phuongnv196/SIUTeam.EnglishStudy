import api from './api'

export interface VocabularyItem {
  id: string
  word: string
  pronunciation: string
  meaning: string
  example: string
  level: string
  topic: string
  learned: boolean
  createdAt?: string
  updatedAt?: string
}

export interface CreateVocabularyItemDto {
  word: string
  pronunciation: string
  meaning: string
  example: string
  level: string
  topic: string
  learned?: boolean
}

export interface UpdateVocabularyItemDto {
  pronunciation?: string
  meaning?: string
  example?: string
  level?: string
  topic?: string
  learned?: boolean
}

export interface VocabularyTopicDto {
  key: string
  label: string
  color: string
  progress: number
  totalWords: number
  learnedWords: number
}

export interface VocabularyStats {
  totalWords: number
  learnedWords: number
  progressPercentage: number
  topicStats: { [topic: string]: number }
}

class VocabularyService {
  private readonly baseUrl = '/vocabulary'

  // Get all vocabulary items
  async getAll(): Promise<VocabularyItem[]> {
    const response = await api.get<VocabularyItem[]>(this.baseUrl)
    return response.data
  }

  // Get vocabulary items by topic
  async getByTopic(topic: string): Promise<VocabularyItem[]> {
    const response = await api.get<VocabularyItem[]>(`${this.baseUrl}/by-topic/${topic}`)
    return response.data
  }

  // Get vocabulary items by level
  async getByLevel(level: string): Promise<VocabularyItem[]> {
    const response = await api.get<VocabularyItem[]>(`${this.baseUrl}/by-level/${level}`)
    return response.data
  }

  // Search vocabulary by word
  async searchByWord(word: string): Promise<VocabularyItem | null> {
    try {
      const response = await api.get<VocabularyItem>(`${this.baseUrl}/search/${word}`)
      return response.data
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null
      }
      throw error
    }
  }

  // Get vocabulary item by ID
  async getById(id: string): Promise<VocabularyItem | null> {
    try {
      const response = await api.get<VocabularyItem>(`${this.baseUrl}/${id}`)
      return response.data
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null
      }
      throw error
    }
  }

  // Create new vocabulary item
  async create(vocabularyItem: CreateVocabularyItemDto): Promise<VocabularyItem> {
    const response = await api.post<VocabularyItem>(this.baseUrl, vocabularyItem)
    return response.data
  }

  // Update vocabulary item
  async update(id: string, updateData: UpdateVocabularyItemDto): Promise<VocabularyItem> {
    const response = await api.put<VocabularyItem>(`${this.baseUrl}/${id}`, updateData)
    return response.data
  }

  // Delete vocabulary item
  async delete(id: string): Promise<boolean> {
    try {
      await api.delete(`${this.baseUrl}/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting vocabulary item:', error)
      return false
    }
  }

  // Mark vocabulary item as learned/unlearned
  async markAsLearned(id: string, learned: boolean = true): Promise<boolean> {
    try {
      await api.patch(`${this.baseUrl}/${id}/learned`, learned)
      return true
    } catch (error) {
      console.error('Error marking vocabulary as learned:', error)
      return false
    }
  }

  // Get all topics with statistics
  async getTopics(): Promise<VocabularyTopicDto[]> {
    const response = await api.get<VocabularyTopicDto[]>(`${this.baseUrl}/topics`)
    return response.data
  }

  // Get vocabulary statistics
  async getStats(): Promise<VocabularyStats> {
    const response = await api.get<{ [key: string]: number }>(`${this.baseUrl}/stats`)
    const stats = response.data
    
    // Calculate total words and learned words from all topics
    let totalWords = 0
    let learnedWords = 0
    
    // Extract topic stats and calculate totals
    const topicStats: { [topic: string]: number } = {}
    Object.keys(stats).forEach(key => {
      if (key.endsWith('_total')) {
        totalWords += stats[key]
      } else if (key.endsWith('_learned')) {
        learnedWords += stats[key]
      }
      topicStats[key] = stats[key]
    })
    
    const progressPercentage = totalWords > 0 ? Math.round((learnedWords / totalWords) * 100) : 0

    return {
      totalWords,
      learnedWords,
      progressPercentage,
      topicStats
    }
  }

  // Get vocabulary levels
  getAvailableLevels(): string[] {
    return ['Beginner', 'Intermediate', 'Advanced']
  }

  // Get vocabulary topics (static list for UI purposes)
  getAvailableTopics(): { key: string; label: string; color: string }[] {
    return [
      { key: 'business', label: 'Business', color: '#1890ff' },
      { key: 'travel', label: 'Travel', color: '#52c41a' },
      { key: 'technology', label: 'Technology', color: '#fa8c16' },
      { key: 'daily', label: 'Daily Life', color: '#eb2f96' },
      { key: 'academic', label: 'Academic', color: '#722ed1' }
    ]
  }
}

const vocabularyService = new VocabularyService()
export default vocabularyService
