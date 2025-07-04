import React, { useEffect, useState } from 'react'
import { 
  Button, 
  Row, 
  Col, 
  Typography, 
  Space, 
  Card, 
  Avatar,
  Rate,
  Carousel,
  Progress,
  Badge
} from 'antd'
import { 
  BookOutlined,
  TrophyOutlined,
  UserOutlined,
  PlayCircleOutlined,
  CheckCircleOutlined,
  GlobalOutlined,
  ClockCircleOutlined,
  ArrowRightOutlined,
  FireOutlined,
  ThunderboltOutlined,
  HeartOutlined,
  RocketOutlined,
  CrownOutlined
} from '@ant-design/icons'
import { useLandingContext } from '../../layouts/LandingLayout/LandingLayout'
import styles from './LandingPage.module.scss'

const { Title, Paragraph } = Typography

const LandingPage: React.FC = () => {
  const [animationClass, setAnimationClass] = useState('')
  const { handleStartLearning } = useLandingContext()

  useEffect(() => {
    const timer = setTimeout(() => {
      setAnimationClass(styles.animateIn)
    }, 100)
    return () => clearTimeout(timer)
  }, [])

  const features = [
    {
      icon: <RocketOutlined className={styles.featureIcon} />,
      title: "AI Phát âm thông minh",
      description: "Công nghệ AI phân tích và chấm điểm phát âm theo chuẩn quốc tế",
      color: "#667eea"
    },
    {
      icon: <FireOutlined className={styles.featureIcon} />,
      title: "Ngữ pháp tương tác",
      description: "Học ngữ pháp qua game và bài tập tương tác thông minh",
      color: "#f093fb"
    },
    {
      icon: <ThunderboltOutlined className={styles.featureIcon} />,
      title: "Giao tiếp theo chủ đề",
      description: "Luyện hội thoại AI với các tình huống thực tế đa dạng",
      color: "#52c41a"
    },
    {
      icon: <CrownOutlined className={styles.featureIcon} />,
      title: "Lộ trình CEFR chuẩn",
      description: "Học theo khung tham chiếu châu Âu từ A1 đến C2",
      color: "#fa8c16"
    }
  ]

  const achievements = [
    {
      icon: <UserOutlined />,
      number: "50,000+",
      label: "Người học miễn phí",
      color: "#1890ff"
    },
    {
      icon: <BookOutlined />,
      number: "100+",
      label: "Chủ đề giao tiếp",
      color: "#52c41a"
    },
    {
      icon: <TrophyOutlined />,
      number: "6",
      label: "Trình độ CEFR",
      color: "#faad14"
    },
    {
      icon: <GlobalOutlined />,
      number: "24/7",
      label: "Hỗ trợ AI miễn phí",
      color: "#eb2f96"
    }
  ]

  const testimonials = [
    {
      name: "Trần Minh Thư",
      role: "Sinh viên IT",
      avatar: "https://images.unsplash.com/photo-1494790108755-2616b612b786?w=150&h=150&fit=crop&crop=face",
      rating: 5,
      comment: "AI phát âm giúp tôi cải thiện accent rất nhiều! Hoàn toàn miễn phí và hiệu quả hơn cả app trả phí."
    },
    {
      name: "Nguyễn Đức Mạnh",
      role: "Nhân viên Marketing",
      avatar: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face",
      rating: 5,
      comment: "Luyện giao tiếp theo chủ đề rất thực tế. AI coach như một người bạn thật sự!"
    },
    {
      name: "Lê Thị Hương",
      role: "Giáo viên",
      avatar: "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face",
      rating: 5,
      comment: "Từ A2 lên B1 chỉ trong 3 tháng! Lộ trình CEFR chuẩn và hoàn toàn miễn phí."
    }
  ]

  const learningFeatures = [
    {
      title: "Luyện phát âm với AI",
      level: "Tất cả trình độ",
      description: "AI phân tích và chữa phát âm theo chuẩn quốc tế",
      topics: ["Âm đơn & âm đôi", "Trọng âm từ & câu", "Ngữ điệu tự nhiên"],
      icon: "🎯",
      color: "#667eea"
    },
    {
      title: "Ngữ pháp tương tác", 
      level: "A1 → C2",
      description: "Học ngữ pháp qua game và bài tập thông minh",
      topics: ["Thì động từ", "Câu điều kiện", "Câu bị động"],
      icon: "📚",
      color: "#52c41a"
    },
    {
      title: "Giao tiếp thực tế",
      level: "B1 → C2",
      description: "Luyện hội thoại AI với 100+ chủ đề đa dạng",
      topics: ["Du lịch", "Công việc", "Cuộc sống hàng ngày"],
      icon: "💬",
      color: "#f093fb"
    }
  ]

  return (
    <div className={styles.landingPage}>
      {/* Hero Section */}
      <section className={styles.heroSection}>
        <div className={styles.heroBackground}>
          <div className={styles.floatingShapes}>
            <div className={`${styles.shape} ${styles.shape1}`}></div>
            <div className={`${styles.shape} ${styles.shape2}`}></div>
            <div className={`${styles.shape} ${styles.shape3}`}></div>
            <div className={`${styles.shape} ${styles.shape4}`}></div>
          </div>
        </div>
        
        <div className={styles.container}>
          <Row align="middle" gutter={[48, 48]}>
            <Col xs={24} lg={12}>
              <div className={`${styles.heroContent} ${animationClass}`}>
                <div className={styles.heroTag}>
                  <FireOutlined /> <span>100% Miễn phí với AI</span>
                </div>
                <Title level={1} className={styles.heroTitle}>
                  Học tiếng Anh miễn phí với
                  <br />
                  <span className={styles.gradientText}>AI Personal Coach</span>
                </Title>
                <Paragraph className={styles.heroDescription}>
                  Nền tảng học tiếng Anh miễn phí hoàn toàn với AI thông minh. 
                  Luyện phát âm, ngữ pháp, giao tiếp theo trình độ CEFR từ A1 đến C2. 
                  Không quảng cáo, không phí ẩn, chỉ cần đăng ký và học ngay!
                </Paragraph>
                
                <div className={styles.heroStats}>
                  <div className={styles.statItem}>
                    <span className={styles.statNumber}>100%</span>
                    <span className={styles.statLabel}>Miễn phí hoàn toàn</span>
                  </div>
                  <div className={styles.statItem}>
                    <span className={styles.statNumber}>50K+</span>
                    <span className={styles.statLabel}>Người học</span>
                  </div>
                  <div className={styles.statItem}>
                    <span className={styles.statNumber}>24/7</span>
                    <span className={styles.statLabel}>AI hỗ trợ</span>
                  </div>
                </div>

                <Space size="large" className={styles.heroCTA}>
                  <Button 
                    type="primary" 
                    size="large" 
                    className={`${styles.primaryButton} ${styles.pulseButton}`}
                    icon={<RocketOutlined />}
                    onClick={handleStartLearning}
                  >
                    Bắt đầu học miễn phí
                  </Button>
                  <Button 
                    size="large" 
                    className={styles.secondaryButton}
                    icon={<PlayCircleOutlined />}
                  >
                    Xem AI hoạt động
                  </Button>
                </Space>
              </div>
            </Col>
            
            <Col xs={24} lg={12}>
              <div className={styles.heroVisual}>
                <div className={styles.dashboardPreview}>
                  <Card className={styles.previewCard}>
                    <div className={styles.cardHeader}>
                      <Avatar size="small" className={styles.aiAvatar}>AI</Avatar>
                      <span>AI English Coach - Miễn phí</span>
                      <Badge status="processing" text="Online 24/7" />
                    </div>
                    <div className={styles.conversationPreview}>
                      <div className={styles.messageBot}>
                        <Avatar size="small" className={styles.botAvatar}>🤖</Avatar>
                        <div className={styles.messageBubble}>
                          Hi! Chọn trình độ của bạn: A1, A2, B1, B2, C1, C2?
                        </div>
                      </div>
                      <div className={styles.messageUser}>
                        <div className={styles.messageBubble}>
                          B1 - Intermediate
                        </div>
                        <Avatar size="small" src="https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=32&h=32&fit=crop&crop=face" />
                      </div>
                      <div className={styles.messageBot}>
                        <Avatar size="small" className={styles.botAvatar}>🤖</Avatar>
                        <div className={styles.messageBubble}>
                          Tuyệt! Hãy luyện phát âm: "I would like to travel" 🎯
                        </div>
                      </div>
                      <div className={styles.messageUser}>
                        <div className={styles.messageBubble}>
                          🎤 [Recording...]
                        </div>
                        <Avatar size="small" src="https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=32&h=32&fit=crop&crop=face" />
                      </div>
                      <div className={styles.messageBot}>
                        <Avatar size="small" className={styles.botAvatar}>🤖</Avatar>
                        <div className={styles.messageBubble}>
                          Xuất sắc! 95/100 điểm! 🌟 Miễn phí mãi mãi!
                        </div>
                      </div>
                    </div>
                  </Card>
                  
                  <Card className={`${styles.progressCard} ${styles.floatingCard1}`}>
                    <div className={styles.progressHeader}>
                      <TrophyOutlined className={styles.progressIcon} />
                      <span>Tiến độ CEFR (Miễn phí)</span>
                    </div>
                    <div className={styles.progressItems}>
                      <div className={styles.progressItem}>
                        <span>Phát âm</span>
                        <Progress percent={85} strokeColor="#52c41a" size="small" />
                      </div>
                      <div className={styles.progressItem}>
                        <span>Ngữ pháp</span>
                        <Progress percent={92} strokeColor="#1890ff" size="small" />
                      </div>
                      <div className={styles.progressItem}>
                        <span>Giao tiếp</span>
                        <Progress percent={78} strokeColor="#faad14" size="small" />
                      </div>
                      <div style={{textAlign: 'center', marginTop: '12px', fontSize: '12px', color: '#52c41a', fontWeight: 600}}>
                        Từ A2 → B1 (3 tháng)
                      </div>
                    </div>
                  </Card>
                </div>
              </div>
            </Col>
          </Row>
        </div>
      </section>

      {/* Achievement Stats Section */}
      <section className={styles.achievementSection}>
        <div className={styles.container}>
          <Row gutter={[32, 32]} justify="center">
            {achievements.map((achievement, index) => (
              <Col xs={12} sm={6} key={index}>
                <div className={`${styles.achievementCard} ${styles.animateUp}`} style={{animationDelay: `${index * 0.1}s`}}>
                  <div className={styles.achievementIcon} style={{backgroundColor: achievement.color}}>
                    {achievement.icon}
                  </div>
                  <div className={styles.achievementNumber}>{achievement.number}</div>
                  <div className={styles.achievementLabel}>{achievement.label}</div>
                </div>
              </Col>
            ))}
          </Row>
        </div>
      </section>

      {/* Features Section */}
      <section className={styles.featuresSection}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <Title level={2} className={styles.sectionTitle}>
              Tại sao chọn nền tảng miễn phí của chúng tôi?
            </Title>
            <Paragraph className={styles.sectionDescription}>
              Học tiếng Anh hoàn toàn miễn phí với công nghệ AI hàng đầu
            </Paragraph>
          </div>
          
          <Row gutter={[32, 32]}>
            {features.map((feature, index) => (
              <Col xs={24} sm={12} lg={6} key={index}>
                <Card 
                  className={`${styles.featureCard} ${styles.hoverCard}`}
                  hoverable
                  style={{animationDelay: `${index * 0.15}s`}}
                >
                  <div className={styles.featureContent}>
                    <div 
                      className={styles.featureIconWrapper}
                      style={{background: `linear-gradient(135deg, ${feature.color}, ${feature.color}aa)`}}
                    >
                      {feature.icon}
                    </div>
                    <Title level={4} className={styles.featureTitle}>{feature.title}</Title>
                    <Paragraph className={styles.featureDescription}>{feature.description}</Paragraph>
                  </div>
                </Card>
              </Col>
            ))}
          </Row>
        </div>
      </section>

      {/* Learning Features Section */}
      <section className={styles.coursesSection}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <Title level={2} className={styles.sectionTitle}>
              Tính năng học tập miễn phí
            </Title>
            <Paragraph className={styles.sectionDescription}>
              Tất cả đều miễn phí 100% - Không phí ẩn, không quảng cáo
            </Paragraph>
          </div>
          
          <Row gutter={[24, 24]}>
            {learningFeatures.map((feature, index) => (
              <Col xs={24} md={8} key={index}>
                <Card 
                  className={`${styles.courseCard} ${styles.premiumCard}`}
                  hoverable
                  cover={
                    <div className={styles.courseCover} style={{background: `linear-gradient(135deg, ${feature.color}, ${feature.color}aa)`}}>
                      <div className={styles.courseBadge}>
                        <Badge.Ribbon text="MIỄN PHÍ" color="green">
                          <div className={styles.courseImage}>
                            <div className={styles.courseIcon} style={{fontSize: '4rem'}}>
                              {feature.icon}
                            </div>
                          </div>
                        </Badge.Ribbon>
                      </div>
                    </div>
                  }
                >
                  <div className={styles.courseInfo}>
                    <div className={styles.courseHeader}>
                      <Title level={4} className={styles.courseTitle}>{feature.title}</Title>
                      <div className={styles.courseRating}>
                        <Rate disabled defaultValue={5} />
                        <span className={styles.ratingText}>5.0</span>
                      </div>
                    </div>
                    
                    <div className={styles.courseMeta}>
                      <span className={`${styles.levelTag} ${styles.all}`}>
                        {feature.level}
                      </span>
                      <span className={styles.duration}>
                        <ClockCircleOutlined /> Học mọi lúc
                      </span>
                    </div>
                    
                    <Paragraph className={styles.featureDescription} style={{marginBottom: '16px', color: 'var(--text-secondary)'}}>
                      {feature.description}
                    </Paragraph>
                    
                    <div className={styles.courseFeatures}>
                      {feature.topics.map((topic, idx) => (
                        <div key={idx} className={styles.featureItem}>
                          <CheckCircleOutlined className={styles.checkIcon} />
                          <span>{topic}</span>
                        </div>
                      ))}
                    </div>
                    
                    <div className={styles.courseStats}>
                      <div className={styles.statItem}>
                        <UserOutlined />
                        <span>Không giới hạn người dùng</span>
                      </div>
                    </div>
                    
                    <div className={styles.coursePrice}>
                      <div className={styles.priceInfo}>
                        <span className={styles.currentPrice} style={{color: '#52c41a', fontSize: '1.5rem', fontWeight: 700}}>
                          MIỄN PHÍ
                        </span>
                      </div>
                      <Button 
                        type="primary" 
                        size="large" 
                        block 
                        className={styles.enrollButton}
                        style={{background: feature.color, borderColor: feature.color}}
                        icon={<ArrowRightOutlined />}
                      >
                        Bắt đầu ngay
                      </Button>
                    </div>
                  </div>
                </Card>
              </Col>
            ))}
          </Row>
          
          <div className={styles.viewAllCourses}>
            <Button size="large" className={styles.outlineButton}>
              Khám phá thêm tính năng
              <ArrowRightOutlined />
            </Button>
          </div>
        </div>
      </section>

      {/* CEFR Levels Section */}
      <section className={styles.levelSection} style={{padding: '80px 0', background: 'var(--bg-secondary)'}}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <Title level={2} className={styles.sectionTitle}>
              Lộ trình học theo chuẩn CEFR
            </Title>
            <Paragraph className={styles.sectionDescription}>
              Từ người mới bắt đầu (A1) đến trình độ thành thạo (C2)
            </Paragraph>
          </div>
          
          <Row gutter={[16, 16]}>
            {[
              {level: 'A1', name: 'Beginner', desc: 'Giao tiếp cơ bản', color: '#ff6b6b'},
              {level: 'A2', name: 'Elementary', desc: 'Tình huống thường ngày', color: '#ffa726'},
              {level: 'B1', name: 'Intermediate', desc: 'Tự tin giao tiếp', color: '#66bb6a'},
              {level: 'B2', name: 'Upper-Int', desc: 'Thảo luận phức tạp', color: '#42a5f5'},
              {level: 'C1', name: 'Advanced', desc: 'Gần như bản ngữ', color: '#ab47bc'},
              {level: 'C2', name: 'Proficiency', desc: 'Thành thạo hoàn toàn', color: '#5c6bc0'}
            ].map((item, index) => (
              <Col xs={12} sm={8} md={4} key={index}>
                <Card 
                  className={styles.levelCard} 
                  style={{
                    textAlign: 'center', 
                    border: `2px solid ${item.color}`,
                    background: `${item.color}15`
                  }}
                >
                  <div style={{
                    fontSize: '2rem', 
                    fontWeight: 700, 
                    color: item.color,
                    marginBottom: '8px'
                  }}>
                    {item.level}
                  </div>
                  <div style={{fontWeight: 600, marginBottom: '4px'}}>{item.name}</div>
                  <div style={{fontSize: '12px', color: 'var(--text-secondary)'}}>{item.desc}</div>
                </Card>
              </Col>
            ))}
          </Row>
        </div>
      </section>

      {/* Testimonials Section */}
      <section className={styles.testimonialsSection}>
        <div className={styles.container}>
          <div className={styles.sectionHeader}>
            <Title level={2} className={styles.sectionTitle}>
              Câu chuyện thành công
            </Title>
            <Paragraph className={styles.sectionDescription}>
              Hàng nghìn học viên đã thay đổi cuộc đời với English Study
            </Paragraph>
          </div>
          
          <Carousel 
            autoplay 
            dots={{ className: styles.testimonialDots }}
            autoplaySpeed={5000}
            effect="fade"
          >
            {testimonials.map((testimonial, index) => (
              <div key={index}>
                <Card className={styles.testimonialCard}>
                  <div className={styles.testimonialContent}>
                    <div className={styles.quoteIcon}>"</div>
                    <Paragraph className={styles.testimonialText}>
                      {testimonial.comment}
                    </Paragraph>
                    <Rate disabled defaultValue={testimonial.rating} className={styles.testimonialRating} />
                    <div className={styles.testimonialAuthor}>
                      <Avatar 
                        src={testimonial.avatar} 
                        size={60} 
                        className={styles.authorAvatar}
                      />
                      <div className={styles.authorInfo}>
                        <Title level={5} className={styles.authorName}>{testimonial.name}</Title>
                        <Paragraph className={styles.authorRole}>{testimonial.role}</Paragraph>
                      </div>
                      <div className={styles.successBadge}>
                        <TrophyOutlined /> Thành công
                      </div>
                    </div>
                  </div>
                </Card>
              </div>
            ))}
          </Carousel>
        </div>
      </section>

      {/* CTA Section */}
      <section className={styles.ctaSection}>
        <div className={styles.ctaBackground}>
          <div className={styles.ctaShapes}>
            <div className={`${styles.ctaShape} ${styles.ctaShape1}`}></div>
            <div className={`${styles.ctaShape} ${styles.ctaShape2}`}></div>
          </div>
        </div>
        
        <div className={styles.container}>
          <div className={styles.ctaContent}>
            <div className={styles.ctaHeader}>
              <Title level={2} className={styles.ctaTitle}>
                Sẵn sàng học tiếng Anh miễn phí?
              </Title>
              <Paragraph className={styles.ctaDescription}>
                Tham gia ngay hôm nay! <strong>100% miễn phí</strong> - 
                Không cần thẻ tín dụng, không phí ẩn, không quảng cáo!
              </Paragraph>
            </div>
            
            <div className={styles.ctaFeatures}>
              <div className={styles.ctaFeature}>
                <CheckCircleOutlined className={styles.ctaFeatureIcon} />
                <span>Hoàn toàn miễn phí</span>
              </div>
              <div className={styles.ctaFeature}>
                <CheckCircleOutlined className={styles.ctaFeatureIcon} />
                <span>AI coach cá nhân</span>
              </div>
              <div className={styles.ctaFeature}>
                <CheckCircleOutlined className={styles.ctaFeatureIcon} />
                <span>Không quảng cáo</span>
              </div>
            </div>
            
            <div className={styles.ctaActions}>
              <Button 
                type="primary" 
                size="large" 
                className={`${styles.ctaPrimaryButton} ${styles.glowButton}`}
                icon={<RocketOutlined />}
                onClick={handleStartLearning}
              >
                Đăng ký miễn phí ngay
              </Button>
              <Button 
                size="large" 
                className={styles.ctaSecondaryButton}
                icon={<HeartOutlined />}
              >
                Tìm hiểu thêm
              </Button>
            </div>
            
            <div className={styles.trustSignals}>
              <span className={styles.trustText}>Được tin tưởng bởi:</span>
              <div className={styles.trustLogos}>
                <span>� 50,000+ Học viên</span>
                <span>� Hoàn toàn miễn phí</span>
                <span>🌟 Không quảng cáo</span>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}

export default LandingPage
