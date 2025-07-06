import React, { useState, useEffect } from 'react'
import { Card, Button, Space, Typography, Progress, Modal, message, Tag } from 'antd'
import { 
  LeftOutlined, 
  RightOutlined, 
  EyeOutlined, 
  SoundOutlined,
  CheckOutlined,
  CloseOutlined,
  ReloadOutlined,
  BookOutlined
} from '@ant-design/icons'
import type { VocabularyItem } from '../../services/vocabularyService'
import textToSpeechService from '../../services/textToSpeechService'
import './FlashcardStudy.css'

const { Title, Text } = Typography

interface FlashcardStudyProps {
  vocabularyItems: VocabularyItem[]
  topic: string
  onClose: () => void
  onMarkAsLearned: (item: VocabularyItem) => void
}

const FlashcardStudy: React.FC<FlashcardStudyProps> = ({
  vocabularyItems,
  topic,
  onClose,
  onMarkAsLearned
}) => {
  const [currentIndex, setCurrentIndex] = useState(0)
  const [isFlipped, setIsFlipped] = useState(false)
  const [studiedItems, setStudiedItems] = useState<Set<string>>(new Set())
  const [correctAnswers, setCorrectAnswers] = useState<Set<string>>(new Set())
  const [showResults, setShowResults] = useState(false)
  const [studyMode, setStudyMode] = useState<'word-to-meaning' | 'meaning-to-word'>('word-to-meaning')
  const [quizMode, setQuizMode] = useState<'self-check' | 'multiple-choice'>('self-check')
  const [selectedAnswer, setSelectedAnswer] = useState<string | null>(null)
  const [showAnswer, setShowAnswer] = useState(false)
  const [multipleChoices, setMultipleChoices] = useState<string[]>([])

  const currentItem = vocabularyItems[currentIndex]
  const progress = ((currentIndex + 1) / vocabularyItems.length) * 100
  const studiedCount = studiedItems.size
  const correctCount = correctAnswers.size

  // Generate multiple choice options
  const generateMultipleChoices = (correctItem: VocabularyItem, allItems: VocabularyItem[]) => {
    const correctAnswer = studyMode === 'word-to-meaning' ? correctItem.meaning : correctItem.word
    const otherItems = allItems.filter(item => item.id !== correctItem.id)
    
    // Get 3 random wrong answers
    const wrongAnswers = otherItems
      .sort(() => Math.random() - 0.5)
      .slice(0, 3)
      .map(item => studyMode === 'word-to-meaning' ? item.meaning : item.word)
    
    // Combine and shuffle
    const choices = [correctAnswer, ...wrongAnswers].sort(() => Math.random() - 0.5)
    return choices
  }

  useEffect(() => {
    // Reset when vocabulary items change
    setCurrentIndex(0)
    setIsFlipped(false)
    setStudiedItems(new Set())
    setCorrectAnswers(new Set())
    setShowResults(false)
    setSelectedAnswer(null)
    setShowAnswer(false)
  }, [vocabularyItems])

  // Generate multiple choices when current item or study mode changes
  useEffect(() => {
    if (currentItem && quizMode === 'multiple-choice') {
      const choices = generateMultipleChoices(currentItem, vocabularyItems)
      setMultipleChoices(choices)
      setSelectedAnswer(null)
      setShowAnswer(false)
    }
  }, [currentIndex, studyMode, quizMode, currentItem, vocabularyItems])

  const handleNext = () => {
    if (currentIndex < vocabularyItems.length - 1) {
      setCurrentIndex(currentIndex + 1)
      setIsFlipped(false) // Reset flip state
      setSelectedAnswer(null)
      setShowAnswer(false)
    } else {
      setShowResults(true)
    }
  }

  const handlePrevious = () => {
    if (currentIndex > 0) {
      setCurrentIndex(currentIndex - 1)
      setIsFlipped(false) // Reset flip state
      setSelectedAnswer(null)
      setShowAnswer(false)
    }
  }

  // Reset flip state when current item changes
  useEffect(() => {
    setIsFlipped(false)
    setSelectedAnswer(null)
    setShowAnswer(false)
  }, [currentIndex])

  const handleFlip = () => {
    if (quizMode === 'self-check') {
      setIsFlipped(!isFlipped)
      if (!isFlipped) {
        setStudiedItems(prev => new Set([...prev, currentItem.id]))
      }
    }
  }

  const handleMultipleChoiceAnswer = (answer: string) => {
    setSelectedAnswer(answer)
    setShowAnswer(true)
    setStudiedItems(prev => new Set([...prev, currentItem.id]))
    
    const correctAnswer = studyMode === 'word-to-meaning' ? currentItem.meaning : currentItem.word
    if (answer === correctAnswer) {
      setCorrectAnswers(prev => new Set([...prev, currentItem.id]))
      message.success('Chính xác! 🎉')
    } else {
      message.error('Sai rồi! Hãy ghi nhớ đáp án đúng.')
    }
  }

  const handleCorrect = () => {
    setCorrectAnswers(prev => new Set([...prev, currentItem.id]))
    setStudiedItems(prev => new Set([...prev, currentItem.id]))
    message.success('Chính xác! 🎉')
    setTimeout(handleNext, 500)
  }

  const handleIncorrect = () => {
    setStudiedItems(prev => new Set([...prev, currentItem.id]))
    message.info('Hãy xem lại từ này!')
    setTimeout(handleNext, 500)
  }

  const handlePlayPronunciation = async () => {
    try {
      await textToSpeechService.speakWithBrowser(currentItem.word)
    } catch (error) {
      console.error('Error playing pronunciation:', error)
      message.error('Không thể phát âm')
    }
  }

  const handleRestart = () => {
    setCurrentIndex(0)
    setIsFlipped(false)
    setStudiedItems(new Set())
    setCorrectAnswers(new Set())
    setShowResults(false)
    setSelectedAnswer(null)
    setShowAnswer(false)
  }

  const handleToggleStudyMode = () => {
    setStudyMode(prev => prev === 'word-to-meaning' ? 'meaning-to-word' : 'word-to-meaning')
    setIsFlipped(false)
    setSelectedAnswer(null)
    setShowAnswer(false)
  }

  if (!currentItem) {
    return null
  }

  if (showResults) {
    const accuracy = vocabularyItems.length > 0 ? Math.round((correctCount / vocabularyItems.length) * 100) : 0
    
    return (
      <Modal
        open={true}
        onCancel={onClose}
        footer={null}
        width={600}
        title={
          <Space>
            <BookOutlined />
            <span>Kết quả học tập - {topic}</span>
          </Space>
        }
      >
        <div style={{ textAlign: 'center', padding: '20px' }}>
          <Title level={2}>🎉 Hoàn thành!</Title>
          
          <div style={{ margin: '20px 0' }}>
            <div style={{ fontSize: '48px', fontWeight: 'bold', color: accuracy >= 80 ? '#52c41a' : accuracy >= 60 ? '#fa8c16' : '#ff4d4f' }}>
              {accuracy}%
            </div>
            <Text type="secondary">Độ chính xác</Text>
          </div>

          <div style={{ margin: '20px 0' }}>
            <Space size="large">
              <div>
                <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#52c41a' }}>
                  {correctCount}
                </div>
                <div>Đúng</div>
              </div>
              <div>
                <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#ff4d4f' }}>
                  {vocabularyItems.length - correctCount}
                </div>
                <div>Sai</div>
              </div>
              <div>
                <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#1890ff' }}>
                  {vocabularyItems.length}
                </div>
                <div>Tổng</div>
              </div>
            </Space>
          </div>

          <Space>
            <Button 
              type="primary" 
              icon={<ReloadOutlined />}
              onClick={handleRestart}
            >
              Học lại
            </Button>
            <Button onClick={onClose}>
              Hoàn thành
            </Button>
          </Space>
        </div>
      </Modal>
    )
  }

  return (
    <Modal
      open={true}
      onCancel={onClose}
      footer={null}
      width={800}
      title={
        <Space>
          <BookOutlined />
          <span>Flashcard - {topic}</span>
          <Tag color="blue">{studyMode === 'word-to-meaning' ? 'Từ → Nghĩa' : 'Nghĩa → Từ'}</Tag>
          <Tag color={quizMode === 'multiple-choice' ? 'green' : 'orange'}>
            {quizMode === 'multiple-choice' ? 'Trắc nghiệm' : 'Tự đánh giá'}
          </Tag>
        </Space>
      }
    >
      <div style={{ padding: '20px' }}>
        {/* Progress */}
        <div className="study-progress">
          <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
            <Text>Tiến độ: {currentIndex + 1}/{vocabularyItems.length}</Text>
            <Text>Đã học: {studiedCount} | Đúng: {correctCount}</Text>
          </div>
          <Progress percent={progress} strokeColor="#1890ff" />
        </div>

        {/* Mode Controls */}
        <div style={{ textAlign: 'center', marginBottom: '20px' }}>
          <Space>
            <Button onClick={handleToggleStudyMode}>
              Chuyển sang {studyMode === 'word-to-meaning' ? 'Nghĩa → Từ' : 'Từ → Nghĩa'}
            </Button>
            <Button 
              onClick={() => setQuizMode(quizMode === 'self-check' ? 'multiple-choice' : 'self-check')}
              type={quizMode === 'multiple-choice' ? 'primary' : 'default'}
            >
              {quizMode === 'self-check' ? '🔄 Chuyển sang Trắc nghiệm' : '🔄 Chuyển sang Tự đánh giá'}
            </Button>
          </Space>
        </div>

        {/* Flashcard */}
        {quizMode === 'self-check' ? (
          // Self-check mode (original flashcard with flip)
          <div style={{ display: 'flex', justifyContent: 'center', marginBottom: '20px' }}>
            <div className="flashcard" style={{ width: '400px', height: '250px' }}>
              <div className={`flashcard-inner ${isFlipped ? 'flipped' : ''}`}>
                <div className="flashcard-front">
                  <Card
                    style={{
                      width: '400px',
                      height: '250px',
                      cursor: 'pointer'
                    }}
                    onClick={handleFlip}
                    bodyStyle={{
                      height: '100%',
                      padding: 0
                    }}
                  >
                    <div className="flashcard-content">
                      <div style={{ fontSize: '12px', color: '#999', marginBottom: '10px' }}>
                        {studyMode === 'word-to-meaning' ? 'ENGLISH WORD' : 'VIETNAMESE MEANING'}
                      </div>
                      <div className={studyMode === 'word-to-meaning' ? 'word-display' : 'meaning-display'}>
                        {studyMode === 'word-to-meaning' ? currentItem.word : currentItem.meaning}
                      </div>
                      {studyMode === 'word-to-meaning' && (
                        <div className="pronunciation-display">
                          {currentItem.pronunciation}
                        </div>
                      )}
                      <Tag 
                        className="level-tag"
                        color={currentItem.level === 'Advanced' ? 'red' : 'blue'}
                      >
                        {currentItem.level}
                      </Tag>
                      <div className="study-hint">
                        👆 Click để xem {studyMode === 'word-to-meaning' ? 'nghĩa tiếng Việt' : 'từ tiếng Anh'}
                      </div>
                    </div>
                  </Card>
                </div>
                
                <div className="flashcard-back">
                  <Card
                    style={{
                      width: '400px',
                      height: '250px',
                      cursor: 'pointer'
                    }}
                    onClick={handleFlip}
                    bodyStyle={{
                      height: '100%',
                      padding: 0
                    }}
                  >
                    <div className="flashcard-content">
                      <div style={{ fontSize: '12px', color: '#999', marginBottom: '10px' }}>
                        {studyMode === 'word-to-meaning' ? 'VIETNAMESE MEANING' : 'ENGLISH WORD'}
                      </div>
                      <div className={studyMode === 'word-to-meaning' ? 'meaning-display' : 'word-display'}>
                        {studyMode === 'word-to-meaning' ? currentItem.meaning : currentItem.word}
                      </div>
                      {studyMode === 'meaning-to-word' && (
                        <div className="pronunciation-display">
                          {currentItem.pronunciation}
                        </div>
                      )}
                      <div className="example-display">
                        "{currentItem.example}"
                      </div>
                      <Space>
                        <Button 
                          type="primary" 
                          icon={<CheckOutlined />}
                          onClick={(e) => {
                            e.stopPropagation()
                            handleCorrect()
                          }}
                          style={{ backgroundColor: '#52c41a', borderColor: '#52c41a' }}
                        >
                          Đúng
                        </Button>
                        <Button 
                          danger 
                          icon={<CloseOutlined />}
                          onClick={(e) => {
                            e.stopPropagation()
                            handleIncorrect()
                          }}
                        >
                          Sai
                        </Button>
                      </Space>
                    </div>
                  </Card>
                </div>
              </div>
            </div>
          </div>
        ) : (
          // Multiple choice mode
          <div style={{ display: 'flex', justifyContent: 'center', marginBottom: '20px' }}>
            <Card style={{ width: '500px', minHeight: '300px' }}>
              <div style={{ textAlign: 'center' }}>
                <div style={{ fontSize: '12px', color: '#999', marginBottom: '10px' }}>
                  {studyMode === 'word-to-meaning' ? 'ENGLISH WORD' : 'VIETNAMESE MEANING'}
                </div>
                <div className={studyMode === 'word-to-meaning' ? 'word-display' : 'meaning-display'}>
                  {studyMode === 'word-to-meaning' ? currentItem.word : currentItem.meaning}
                </div>
                {studyMode === 'word-to-meaning' && (
                  <div className="pronunciation-display">
                    {currentItem.pronunciation}
                  </div>
                )}
                <Tag 
                  className="level-tag"
                  color={currentItem.level === 'Advanced' ? 'red' : 'blue'}
                >
                  {currentItem.level}
                </Tag>
                
                <div style={{ marginTop: '20px' }}>
                  <div style={{ fontSize: '14px', marginBottom: '15px', color: '#666' }}>
                    Chọn {studyMode === 'word-to-meaning' ? 'nghĩa đúng' : 'từ tiếng Anh đúng'}:
                  </div>
                  <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                    {multipleChoices.map((choice, index) => {
                      const correctAnswer = studyMode === 'word-to-meaning' ? currentItem.meaning : currentItem.word
                      const isCorrect = choice === correctAnswer
                      const isSelected = selectedAnswer === choice
                      
                      let buttonType: 'default' | 'primary' | 'dashed' = 'default'
                      let buttonStyle: React.CSSProperties = { width: '100%', textAlign: 'left' }
                      
                      if (showAnswer) {
                        if (isCorrect) {
                          buttonType = 'primary'
                          buttonStyle = { 
                            ...buttonStyle, 
                            backgroundColor: '#52c41a', 
                            borderColor: '#52c41a',
                            color: 'white'
                          }
                        } else if (isSelected) {
                          buttonStyle = { 
                            ...buttonStyle, 
                            backgroundColor: '#ff4d4f', 
                            borderColor: '#ff4d4f',
                            color: 'white'
                          }
                        }
                      } else if (isSelected) {
                        buttonType = 'primary'
                      }
                      
                      return (
                        <Button
                          key={index}
                          type={buttonType}
                          style={buttonStyle}
                          onClick={() => !showAnswer && handleMultipleChoiceAnswer(choice)}
                          disabled={showAnswer}
                        >
                          {String.fromCharCode(65 + index)}. {choice}
                          {showAnswer && isCorrect && ' ✓'}
                          {showAnswer && isSelected && !isCorrect && ' ✗'}
                        </Button>
                      )
                    })}
                  </Space>
                  
                  {showAnswer && (
                    <div style={{ marginTop: '15px', padding: '10px', backgroundColor: '#f0f0f0', borderRadius: '4px' }}>
                      <div style={{ fontSize: '12px', color: '#666', marginBottom: '5px' }}>Ví dụ:</div>
                      <div style={{ fontStyle: 'italic' }}>"{currentItem.example}"</div>
                    </div>
                  )}
                </div>
              </div>
            </Card>
          </div>
        )}

        {/* Controls */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Button 
            icon={<LeftOutlined />}
            onClick={handlePrevious}
            disabled={currentIndex === 0}
          >
            Trước
          </Button>

          <Space>
            <Button 
              icon={<SoundOutlined />}
              onClick={handlePlayPronunciation}
            >
              Phát âm
            </Button>
            
            {quizMode === 'self-check' && (
              <Button 
                icon={<EyeOutlined />}
                onClick={handleFlip}
                type={isFlipped ? 'default' : 'primary'}
              >
                {isFlipped ? 'Ẩn đáp án' : `Hiện ${studyMode === 'word-to-meaning' ? 'nghĩa' : 'từ tiếng Anh'}`}
              </Button>
            )}
            
            <Button 
              onClick={() => onMarkAsLearned(currentItem)}
              type={currentItem.learned ? 'primary' : 'default'}
            >
              {currentItem.learned ? '✓ Đã học' : 'Đánh dấu đã học'}
            </Button>
          </Space>

          <Button 
            icon={<RightOutlined />}
            onClick={handleNext}
            type="primary"
            disabled={quizMode === 'multiple-choice' && !showAnswer}
          >
            {currentIndex === vocabularyItems.length - 1 ? 'Hoàn thành' : 'Tiếp'}
          </Button>
        </div>
      </div>
    </Modal>
  )
}

export default FlashcardStudy
