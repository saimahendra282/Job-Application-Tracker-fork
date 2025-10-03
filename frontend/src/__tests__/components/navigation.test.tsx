import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@/__tests__/utils/test-utils'
import { Navigation } from '@/components/navigation'
import { usePathname } from 'next/navigation'

// Mock next/navigation
vi.mock('next/navigation', async () => {
  const actual = await vi.importActual('next/navigation')
  return {
    ...actual,
    usePathname: vi.fn(),
  }
})

describe('Navigation Component', () => {
  it('renders correctly', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    expect(screen.getByText('Job Tracker')).toBeInTheDocument()
  })

  it('renders all navigation routes', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    expect(screen.getByText('Dashboard')).toBeInTheDocument()
    expect(screen.getByText('Applications')).toBeInTheDocument()
    expect(screen.getByText('Calendar')).toBeInTheDocument()
    expect(screen.getByText('Analytics')).toBeInTheDocument()
  })

  it('highlights active route - Dashboard', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    const dashboardLink = screen.getByText('Dashboard').closest('a')
    expect(dashboardLink).toHaveClass('text-white', 'bg-white/10')
  })

  it('highlights active route - Applications', () => {
    vi.mocked(usePathname).mockReturnValue('/applications')
    
    render(<Navigation />)
    
    const applicationsLink = screen.getByText('Applications').closest('a')
    expect(applicationsLink).toHaveClass('text-white', 'bg-white/10')
  })

  it('highlights active route - Calendar', () => {
    vi.mocked(usePathname).mockReturnValue('/calendar')
    
    render(<Navigation />)
    
    const calendarLink = screen.getByText('Calendar').closest('a')
    expect(calendarLink).toHaveClass('text-white', 'bg-white/10')
  })

  it('highlights active route - Analytics', () => {
    vi.mocked(usePathname).mockReturnValue('/analytics')
    
    render(<Navigation />)
    
    const analyticsLink = screen.getByText('Analytics').closest('a')
    expect(analyticsLink).toHaveClass('text-white', 'bg-white/10')
  })

  it('renders inactive routes with neutral color', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    const applicationsLink = screen.getByText('Applications').closest('a')
    expect(applicationsLink).toHaveClass('text-neutral-400')
  })

  it('renders all route links with correct hrefs', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    expect(screen.getByText('Dashboard').closest('a')).toHaveAttribute('href', '/')
    expect(screen.getByText('Applications').closest('a')).toHaveAttribute('href', '/applications')
    expect(screen.getByText('Calendar').closest('a')).toHaveAttribute('href', '/calendar')
    expect(screen.getByText('Analytics').closest('a')).toHaveAttribute('href', '/analytics')
  })

  it('renders the logo link', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    const logoLink = screen.getByText('Job Tracker').closest('a')
    expect(logoLink).toHaveAttribute('href', '/')
  })

  it('renders ThemeToggle component', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    render(<Navigation />)
    
    // ThemeToggle renders a button with "Toggle theme" sr-only text
    expect(screen.getByText('Toggle theme')).toBeInTheDocument()
  })

  it('has correct layout structure', () => {
    vi.mocked(usePathname).mockReturnValue('/')
    
    const { container } = render(<Navigation />)
    
    const nav = container.firstChild as HTMLElement
    expect(nav).toHaveClass('space-y-4', 'flex', 'flex-col', 'h-full')
  })
})
