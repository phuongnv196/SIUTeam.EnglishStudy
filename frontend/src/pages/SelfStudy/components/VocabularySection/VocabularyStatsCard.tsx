import React from 'react'
import { Card, Row, Col } from 'antd'
import type { VocabularyStats } from '../../../../services/vocabularyService'

interface VocabularyStatsProps {
  stats: VocabularyStats
}

const VocabularyStatsCard: React.FC<VocabularyStatsProps> = ({ stats }) => {
  return (
    <Card title="📊 Thống kê học tập">
      <Row gutter={[16, 16]}>
        <Col xs={24} sm={8}>
          <div style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#1890ff' }}>
              {stats.totalWords}
            </div>
            <div>Tổng từ vựng</div>
          </div>
        </Col>
        <Col xs={24} sm={8}>
          <div style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#52c41a' }}>
              {stats.learnedWords}
            </div>
            <div>Đã học</div>
          </div>
        </Col>
        <Col xs={24} sm={8}>
          <div style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '24px', fontWeight: 'bold', color: '#fa8c16' }}>
              {stats.progressPercentage}%
            </div>
            <div>Tiến độ</div>
          </div>
        </Col>
      </Row>
    </Card>
  )
}

export default VocabularyStatsCard
