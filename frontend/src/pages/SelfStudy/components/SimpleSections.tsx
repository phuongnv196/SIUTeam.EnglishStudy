import React from 'react'
import { Card, Typography, Button, Space } from 'antd'
import { BookOutlined, PlayCircleOutlined } from '@ant-design/icons'

const { Title, Paragraph } = Typography

// Simple placeholder components for testing
export const VocabularySection: React.FC = () => (
  <Card title={<Space><BookOutlined />Từ vựng</Space>}>
    <Paragraph>Học và ôn luyện từ vựng theo chủ đề.</Paragraph>
    <Button type="primary" icon={<PlayCircleOutlined />}>Bắt đầu học</Button>
  </Card>
)

export const GrammarSection: React.FC = () => (
  <Card title="Ngữ pháp">
    <Paragraph>Nắm vững các quy tắc ngữ pháp cơ bản và nâng cao.</Paragraph>
    <Button type="primary">Học ngay</Button>
  </Card>
)

export const ListeningSection: React.FC = () => (
  <Card title="Nghe nói">
    <Paragraph>Luyện nghe và phát âm chuẩn.</Paragraph>
    <Button type="primary">Luyện tập</Button>
  </Card>
)

export const ReadingSection: React.FC = () => (
  <Card title="Đọc">
    <Paragraph>Phát triển kỹ năng đọc hiểu.</Paragraph>
    <Button type="primary">Đọc bài</Button>
  </Card>
)

export const WritingSection: React.FC = () => (
  <Card title="Viết">
    <Paragraph>Luyện tập kỹ năng viết học thuật và giao tiếp.</Paragraph>
    <Button type="primary">Bắt đầu viết</Button>
  </Card>
)

export const ConversationSection: React.FC = () => (
  <Card title="Hội thoại">
    <Paragraph>Thực hành giao tiếp trong các tình huống thực tế.</Paragraph>
    <Button type="primary">Thực hành</Button>
  </Card>
)

export const CombinedPracticeSection: React.FC = () => (
  <Card title="Luyện tập kết hợp">
    <Paragraph>Kết hợp tất cả kỹ năng trong các bài tập tổng hợp.</Paragraph>
    <Button type="primary">Bắt đầu</Button>
  </Card>
)
