import React, { useState, useEffect } from 'react'
import { Typography, Card, Button, Radio, Progress, Space, Result, Tag } from 'antd'
import { 
  ClockCircleOutlined, 
  TrophyOutlined, 
  CheckCircleOutlined,
  WarningOutlined,
  PlayCircleOutlined,
  ReloadOutlined
} from '@ant-design/icons'
import styles from './QuizPage.module.scss'

const { Title, Paragraph, Text } = Typography

interface Question {
  id: number
  question: string
  options: string[]
  correctAnswer: number
  explanation?: string
  difficulty: 'easy' | 'medium' | 'hard'
  skill: 'grammar' | 'vocabulary' | 'reading' | 'listening'
}

interface Quiz {
  id: number
  title: string
  description: string
  questions: Question[]
  timeLimit: number // in minutes
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  skill: string
}

const QuizPage: React.FC = () => {
  const [selectedQuiz, setSelectedQuiz] = useState<Quiz | null>(null)
  const [currentQuestion, setCurrentQuestion] = useState(0)
  const [answers, setAnswers] = useState<number[]>([])
  const [timeLeft, setTimeLeft] = useState(0)
  const [isQuizStarted, setIsQuizStarted] = useState(false)
  const [isQuizCompleted, setIsQuizCompleted] = useState(false)
  const [showResult, setShowResult] = useState(false)
  const [score, setScore] = useState(0)

  // Sample quiz data
  const availableQuizzes: Quiz[] = [
    {
      id: 1,
      title: 'Grammar Fundamentals',
      description: 'Test your understanding of basic English grammar rules including tenses, articles, and sentence structure.',
      timeLimit: 15,
      difficulty: 'beginner',
      skill: 'Grammar',
      questions: [
        {
          id: 1,
          question: 'Which sentence is grammatically correct?',
          options: [
            'I have been living here since 5 years.',
            'I have been living here for 5 years.',
            'I am living here since 5 years.',
            'I live here since 5 years.'
          ],
          correctAnswer: 1,
          explanation: 'We use "for" with a period of time (5 years) and "since" with a point in time.',
          difficulty: 'easy',
          skill: 'grammar'
        },
        {
          id: 2,
          question: 'Choose the correct form: "If I _____ you, I would study harder."',
          options: [
            'am',
            'was',
            'were',
            'will be'
          ],
          correctAnswer: 2,
          explanation: 'In second conditional sentences, we use "were" for all persons after "if".',
          difficulty: 'medium',
          skill: 'grammar'
        },
        {
          id: 3,
          question: 'Select the sentence with correct article usage:',
          options: [
            'The honesty is the best policy.',
            'Honesty is the best policy.',
            'A honesty is the best policy.',
            'An honesty is the best policy.'
          ],
          correctAnswer: 1,
          explanation: 'Abstract nouns like "honesty" don\'t need articles when used in general statements.',
          difficulty: 'medium',
          skill: 'grammar'
        }
      ]
    },
    {
      id: 2,
      title: 'Vocabulary Builder',
      description: 'Expand your vocabulary with commonly used words in academic and professional contexts.',
      timeLimit: 10,
      difficulty: 'intermediate',
      skill: 'Vocabulary',
      questions: [
        {
          id: 1,
          question: 'What does "ubiquitous" mean?',
          options: [
            'Very rare',
            'Extremely large',
            'Present everywhere',
            'Difficult to understand'
          ],
          correctAnswer: 2,
          explanation: 'Ubiquitous means present, appearing, or found everywhere.',
          difficulty: 'hard',
          skill: 'vocabulary'
        },
        {
          id: 2,
          question: 'Choose the synonym for "meticulous":',
          options: [
            'Careless',
            'Detailed',
            'Fast',
            'Simple'
          ],
          correctAnswer: 1,
          explanation: 'Meticulous means showing great attention to detail; very careful and precise.',
          difficulty: 'medium',
          skill: 'vocabulary'
        }
      ]
    },
    {
      id: 3,
      title: 'Reading Comprehension',
      description: 'Test your ability to understand and analyze written English texts.',
      timeLimit: 20,
      difficulty: 'advanced',
      skill: 'Reading',
      questions: [
        {
          id: 1,
          question: 'Based on the passage context, what would be the main theme?',
          options: [
            'Environmental conservation',
            'Technological advancement',
            'Social inequality',
            'Economic development'
          ],
          correctAnswer: 0,
          explanation: 'The passage primarily discusses environmental issues and conservation efforts.',
          difficulty: 'hard',
          skill: 'reading'
        }
      ]
    }
  ]

  // Timer effect
  useEffect(() => {
    let timer: number
    if (isQuizStarted && timeLeft > 0 && !isQuizCompleted) {
      timer = window.setInterval(() => {
        setTimeLeft(prev => {
          if (prev <= 1) {
            handleQuizComplete()
            return 0
          }
          return prev - 1
        })
      }, 1000)
    }
    return () => clearInterval(timer)
  }, [isQuizStarted, timeLeft, isQuizCompleted])

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60)
    const secs = seconds % 60
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
  }

  const startQuiz = (quiz: Quiz) => {
    setSelectedQuiz(quiz)
    setCurrentQuestion(0)
    setAnswers(new Array(quiz.questions.length).fill(-1))
    setTimeLeft(quiz.timeLimit * 60)
    setIsQuizStarted(true)
    setIsQuizCompleted(false)
    setShowResult(false)
  }

  const handleAnswerSelect = (answerIndex: number) => {
    const newAnswers = [...answers]
    newAnswers[currentQuestion] = answerIndex
    setAnswers(newAnswers)
  }

  const goToNextQuestion = () => {
    if (selectedQuiz && currentQuestion < selectedQuiz.questions.length - 1) {
      setCurrentQuestion(prev => prev + 1)
    }
  }

  const goToPreviousQuestion = () => {
    if (currentQuestion > 0) {
      setCurrentQuestion(prev => prev - 1)
    }
  }

  const handleQuizComplete = () => {
    if (!selectedQuiz) return
    
    let correctAnswers = 0
    selectedQuiz.questions.forEach((question, index) => {
      if (answers[index] === question.correctAnswer) {
        correctAnswers++
      }
    })
    
    const finalScore = Math.round((correctAnswers / selectedQuiz.questions.length) * 100)
    setScore(finalScore)
    setIsQuizCompleted(true)
    setShowResult(true)
  }

  const resetQuiz = () => {
    setSelectedQuiz(null)
    setCurrentQuestion(0)
    setAnswers([])
    setTimeLeft(0)
    setIsQuizStarted(false)
    setIsQuizCompleted(false)
    setShowResult(false)
    setScore(0)
  }

  const getDifficultyColor = (difficulty: string) => {
    switch (difficulty) {
      case 'beginner': return 'green'
      case 'intermediate': return 'orange'
      case 'advanced': return 'red'
      default: return 'blue'
    }
  }

  const getScoreColor = (score: number) => {
    if (score >= 90) return '#52c41a'
    if (score >= 70) return '#fa8c16'
    if (score >= 50) return '#faad14'
    return '#ff4d4f'
  }

  if (showResult && selectedQuiz) {
    return (
      <div className={styles.quizContainer}>
        <Result
          icon={<TrophyOutlined style={{ color: getScoreColor(score) }} />}
          title={`Quiz hoàn thành!`}
          subTitle={
            <div>
              <Title level={2} style={{ color: getScoreColor(score), margin: '16px 0' }}>
                Điểm số: {score}/100
              </Title>
              <Paragraph>
                Bạn đã trả lời đúng {selectedQuiz.questions.filter((q, i) => answers[i] === q.correctAnswer).length}/{selectedQuiz.questions.length} câu hỏi
              </Paragraph>
            </div>
          }
          extra={[
            <Button type="primary" key="retry" icon={<ReloadOutlined />} onClick={() => startQuiz(selectedQuiz)}>
              Làm lại
            </Button>,
            <Button key="back" onClick={resetQuiz}>
              Về danh sách Quiz
            </Button>
          ]}
        />
        
        {/* Detailed Results */}
        <Card title="📋 Chi tiết kết quả" style={{ marginTop: '24px' }}>
          {selectedQuiz.questions.map((question, index) => (
            <Card key={question.id} size="small" style={{ marginBottom: '16px' }}>
              <div style={{ marginBottom: '12px' }}>
                <Text strong>Câu {index + 1}: {question.question}</Text>
                {answers[index] === question.correctAnswer ? (
                  <CheckCircleOutlined style={{ color: '#52c41a', marginLeft: '8px' }} />
                ) : (
                  <WarningOutlined style={{ color: '#ff4d4f', marginLeft: '8px' }} />
                )}
              </div>
              <div style={{ marginBottom: '8px' }}>
                <Text>Đáp án của bạn: </Text>
                <Text type={answers[index] === question.correctAnswer ? 'success' : 'danger'}>
                  {answers[index] >= 0 ? question.options[answers[index]] : 'Không trả lời'}
                </Text>
              </div>
              <div style={{ marginBottom: '8px' }}>
                <Text>Đáp án đúng: </Text>
                <Text type="success">{question.options[question.correctAnswer]}</Text>
              </div>
              {question.explanation && (
                <div style={{ backgroundColor: '#f6ffed', padding: '8px', borderRadius: '4px', border: '1px solid #b7eb8f' }}>
                  <Text style={{ fontSize: '12px' }}>💡 {question.explanation}</Text>
                </div>
              )}
            </Card>
          ))}
        </Card>
      </div>
    )
  }

  if (isQuizStarted && selectedQuiz) {
    const currentQ = selectedQuiz.questions[currentQuestion]
    const progress = ((currentQuestion + 1) / selectedQuiz.questions.length) * 100

    return (
      <div className={styles.quizContainer}>
        <Card className={styles.quizHeader}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
            <Title level={3} style={{ margin: 0 }}>{selectedQuiz.title}</Title>
            <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
              <Text><ClockCircleOutlined /> {formatTime(timeLeft)}</Text>
              <Text>Câu {currentQuestion + 1}/{selectedQuiz.questions.length}</Text>
            </div>
          </div>
          <Progress percent={progress} strokeColor="#1890ff" />
        </Card>

        <Card className={styles.questionCard}>
          <Title level={4}>{currentQ.question}</Title>
          <Radio.Group 
            value={answers[currentQuestion]} 
            onChange={(e) => handleAnswerSelect(e.target.value)}
            style={{ width: '100%' }}
          >
            <Space direction="vertical" style={{ width: '100%' }}>
              {currentQ.options.map((option, index) => (
                <Radio key={index} value={index} style={{ padding: '8px', width: '100%' }}>
                  {option}
                </Radio>
              ))}
            </Space>
          </Radio.Group>
        </Card>

        <Card>
          <div style={{ display: 'flex', justifyContent: 'space-between' }}>
            <Button 
              onClick={goToPreviousQuestion} 
              disabled={currentQuestion === 0}
            >
              Câu trước
            </Button>
            <Space>
              {currentQuestion === selectedQuiz.questions.length - 1 ? (
                <Button 
                  type="primary" 
                  onClick={handleQuizComplete}
                  disabled={answers[currentQuestion] === -1}
                >
                  Hoàn thành Quiz
                </Button>
              ) : (
                <Button 
                  type="primary" 
                  onClick={goToNextQuestion}
                  disabled={answers[currentQuestion] === -1}
                >
                  Câu tiếp theo
                </Button>
              )}
            </Space>
          </div>
        </Card>
      </div>
    )
  }

  return (
    <div className={styles.quizContainer}>
      <Title level={2}>📝 Bài kiểm tra</Title>
      <Paragraph>
        Chọn một bài kiểm tra để đánh giá trình độ và củng cố kiến thức của bạn.
      </Paragraph>

      <div className={styles.quizGrid}>
        {availableQuizzes.map(quiz => (
          <Card 
            key={quiz.id}
            className={styles.quizCard}
            hoverable
            actions={[
              <Button 
                type="primary" 
                icon={<PlayCircleOutlined />} 
                onClick={() => startQuiz(quiz)}
              >
                Bắt đầu
              </Button>
            ]}
          >
            <div className={styles.quizCardHeader}>
              <Title level={4} style={{ margin: 0 }}>{quiz.title}</Title>
              <div>
                <Tag color={getDifficultyColor(quiz.difficulty)}>
                  {quiz.difficulty.charAt(0).toUpperCase() + quiz.difficulty.slice(1)}
                </Tag>
                <Tag color="blue">{quiz.skill}</Tag>
              </div>
            </div>
            
            <Paragraph style={{ margin: '12px 0' }}>{quiz.description}</Paragraph>
            
            <div className={styles.quizStats}>
              <div><ClockCircleOutlined /> {quiz.timeLimit} phút</div>
              <div>📝 {quiz.questions.length} câu hỏi</div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  )
}

export default QuizPage
