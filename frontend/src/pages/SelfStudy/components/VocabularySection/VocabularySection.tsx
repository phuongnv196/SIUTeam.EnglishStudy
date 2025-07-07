import React, { useState, useEffect } from 'react'
import { Row, Col, Spin, message } from 'antd'
import vocabularyService from '../../../../services/vocabularyService'
import FlashcardStudy from '../../../../components/Flashcard/FlashcardStudy'
import type { VocabularyItem, VocabularyStats } from '../../../../services/vocabularyService'
import { useSpeakingHub } from '../../../../hooks/useSpeakingHub'

// Import all the components
import {
  APIStatusAlert,
  VocabularyStatsCard,
  SpeakingTestResults,
  TopicSelector,
  VocabularyList,
  VocabularyModal,
  TestModal
} from './index'

const VocabularySection: React.FC = () => {
  const [selectedTopic, setSelectedTopic] = useState('business')
  const [viewModalVisible, setViewModalVisible] = useState(false)
  const [selectedWord, setSelectedWord] = useState<VocabularyItem | null>(null)
  const [testModalVisible, setTestModalVisible] = useState(false)
  
  // API Data State
  const [vocabularyData, setVocabularyData] = useState<VocabularyItem[]>([])
  const [topics, setTopics] = useState<Array<{ key: string; label: string; color: string; progress: number }>>([])
  const [loading, setLoading] = useState(true)
  const [stats, setStats] = useState<VocabularyStats | null>(null)
  
  // Flashcard Study State
  const [showFlashcardStudy, setShowFlashcardStudy] = useState(false)
  const [studyItems, setStudyItems] = useState<VocabularyItem[]>([])

  // Only keep speaking test results state for the main component display
  const [lastTranscription, setLastTranscription] = useState<string>('')

  // Speaking Hub integration - only for getting transcription results
  const {
    pronunciationFeedback,
    clearPronunciationFeedback,
    lastTranscription: hubTranscription
  } = useSpeakingHub()

  // Load initial data
  useEffect(() => {
    loadInitialData()
  }, [])

  // Load vocabulary data when topic changes
  useEffect(() => {
    if (selectedTopic) {
      loadVocabularyByTopic(selectedTopic)
    }
  }, [selectedTopic])

  // Update transcription when hub returns result
  useEffect(() => {
    if (hubTranscription) {
      setLastTranscription(hubTranscription)
      message.success(`Kết quả nhận dạng: "${hubTranscription}"`)
    }
  }, [hubTranscription])

  const loadInitialData = async () => {
    try {
      setLoading(true)
      
      // Load topics and stats
      const [topicsData, statsData] = await Promise.all([
        vocabularyService.getTopics(),
        vocabularyService.getStats()
      ])

      // Convert API topics to UI format
      const staticTopics = vocabularyService.getAvailableTopics()
      const topicsWithProgress = staticTopics.map(staticTopic => {
        const apiTopic = topicsData.find(t => t.key === staticTopic.key)
        const progress = apiTopic ? apiTopic.progress : 0
        return {
          ...staticTopic,
          progress
        }
      })

      setTopics(topicsWithProgress)
      setStats(statsData)
      
      // Load vocabulary for default topic
      await loadVocabularyByTopic(selectedTopic)
      
    } catch (error) {
      console.error('Error loading initial data:', error)
      message.error('Không thể tải dữ liệu từ vựng')
    } finally {
      setLoading(false)
    }
  }

  const loadVocabularyByTopic = async (topic: string) => {
    try {
      const data = await vocabularyService.getByTopic(topic)
      setVocabularyData(data)
    } catch (error) {
      console.error('Error loading vocabulary by topic:', error)
      message.error('Không thể tải từ vựng theo chủ đề')
    }
  }

  const handleMarkAsLearned = async (item: VocabularyItem) => {
    try {
      const newLearnedStatus = !item.learned
      await vocabularyService.markAsLearned(item.id, newLearnedStatus)
      
      // Update local state
      setVocabularyData(prev => 
        prev.map(v => v.id === item.id ? { ...v, learned: newLearnedStatus } : v)
      )
      
      // Update selected word if it's the same item
      if (selectedWord?.id === item.id) {
        setSelectedWord({ ...selectedWord, learned: newLearnedStatus })
      }
      
      // Update study items if flashcard is open
      setStudyItems(prev => 
        prev.map(v => v.id === item.id ? { ...v, learned: newLearnedStatus } : v)
      )
      
      message.success(newLearnedStatus ? 'Đã đánh dấu đã học' : 'Đã bỏ đánh dấu đã học')
      
      // Refresh stats and topics
      const [topicsData, statsData] = await Promise.all([
        vocabularyService.getTopics(),
        vocabularyService.getStats()
      ])
      
      const staticTopics = vocabularyService.getAvailableTopics()
      const topicsWithProgress = staticTopics.map(staticTopic => {
        const apiTopic = topicsData.find(t => t.key === staticTopic.key)
        const progress = apiTopic ? apiTopic.progress : 0
        return {
          ...staticTopic,
          progress
        }
      })
      
      setTopics(topicsWithProgress)
      setStats(statsData)
      
    } catch (error) {
      console.error('Error marking as learned:', error)
      message.error('Không thể cập nhật trạng thái học')
    }
  }

  const startFlashcardStudy = (studyType: 'all' | 'unlearned' = 'all') => {
    const itemsToStudy = studyType === 'unlearned' 
      ? filteredVocab.filter(item => !item.learned)
      : filteredVocab

    if (itemsToStudy.length === 0) {
      message.info('Không có từ vựng nào để học!')
      return
    }

    // Shuffle the items for better learning experience
    const shuffledItems = [...itemsToStudy].sort(() => Math.random() - 0.5)
    setStudyItems(shuffledItems)
    setShowFlashcardStudy(true)
  }

  const closeFlashcardStudy = () => {
    setShowFlashcardStudy(false)
    setStudyItems([])
    // Refresh data after study session
    loadInitialData()
  }

  const handleTopicSelect = (topicKey: string) => {
    setSelectedTopic(topicKey)
  }

  const handleStartFlashcardStudy = (topicKey: string) => {
    setSelectedTopic(topicKey)
    setTimeout(() => startFlashcardStudy('all'), 100)
  }

  const handleViewWord = (word: VocabularyItem) => {
    setSelectedWord(word)
    setViewModalVisible(true)
  }

  const handleSetDebugToken = () => {
    const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicGh1b25nbnYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjllOWRiZDYyLTkwMTQtNDQwMi04NjMxLWYzYmYyMmFjNTNjMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjAiLCJleHAiOjE3NTE4MjMyNDcsImlzcyI6IlNJVVRlYW0uRW5nbGlzaFN0dWR5LkFQSS5ERVYiLCJhdWQiOiJTSVVUZWFtLkVuZ2xpc2hTdHVkeS5DbGllbnQuREVWIn0.vgef50Qrz7No1g-d2ySfFXnKYpqz85QRkKIwWiAEaO0'
    localStorage.setItem('token', token)
    message.success('Debug token set! Refresh để test API.')
  }

  const handleCloseSpeakingResults = () => {
    setLastTranscription('')
    clearPronunciationFeedback()
  }

  const currentTopic = topics.find(t => t.key === selectedTopic)
  const filteredVocab = vocabularyData.filter(item => item.topic === selectedTopic)

  return (
    <div>
      {/* API Status Section */}
      <Row gutter={[16, 16]} style={{ marginBottom: '16px' }}>
        <Col span={24}>
          <APIStatusAlert
            onOpenTestModal={() => setTestModalVisible(true)}
            onRefreshData={loadInitialData}
            loading={loading}
            onSetDebugToken={handleSetDebugToken}
          />
        </Col>
      </Row>

      {/* Speaking Test Results */}
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <SpeakingTestResults
            lastTranscription={lastTranscription}
            pronunciationFeedback={pronunciationFeedback}
            onClose={handleCloseSpeakingResults}
          />
        </Col>
      </Row>

      {loading ? (
        <div style={{ textAlign: 'center', padding: '50px' }}>
          <Spin size="large" />
          <div style={{ marginTop: '16px' }}>Đang tải dữ liệu từ vựng...</div>
        </div>
      ) : (
        <>
          {/* Statistics Section */}
          {stats && (
            <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
              <Col span={24}>
                <VocabularyStatsCard stats={stats} />
              </Col>
            </Row>
          )}

          {/* Topics Section */}
          <TopicSelector
            topics={topics}
            selectedTopic={selectedTopic}
            onTopicSelect={handleTopicSelect}
            onStartFlashcardStudy={handleStartFlashcardStudy}
          />

          {/* Vocabulary List */}
          <VocabularyList
            currentTopic={currentTopic}
            filteredVocab={filteredVocab}
            onStartFlashcardStudy={startFlashcardStudy}
            onViewWord={handleViewWord}
            onMarkAsLearned={handleMarkAsLearned}
          />

          {/* Vocabulary Modal */}
          <VocabularyModal
            visible={viewModalVisible}
            selectedWord={selectedWord}
            onClose={() => setViewModalVisible(false)}
          />

          {/* Test Modal */}
          <TestModal
            visible={testModalVisible}
            onClose={() => setTestModalVisible(false)}
          />
        </>
      )}

      {/* Flashcard Study Modal */}
      {showFlashcardStudy && (
        <FlashcardStudy
          vocabularyItems={studyItems}
          topic={currentTopic?.label || selectedTopic}
          onClose={closeFlashcardStudy}
          onMarkAsLearned={handleMarkAsLearned}
        />
      )}
    </div>
  )
}

export default VocabularySection
