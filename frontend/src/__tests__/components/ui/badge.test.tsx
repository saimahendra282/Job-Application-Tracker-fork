import { describe, it, expect } from 'vitest'
import { render, screen } from '@/__tests__/utils/test-utils'
import { Badge } from '@/components/ui/badge'

describe('Badge Component', () => {
  it('renders correctly with default variant', () => {
    render(<Badge>Default Badge</Badge>)
    
    const badge = screen.getByText('Default Badge')
    expect(badge).toBeInTheDocument()
    expect(badge).toHaveTextContent('Default Badge')
  })

  it('renders with different variants', () => {
    const { rerender } = render(<Badge variant="default">Default</Badge>)
    expect(screen.getByText('Default')).toHaveClass('bg-primary')

    rerender(<Badge variant="secondary">Secondary</Badge>)
    expect(screen.getByText('Secondary')).toHaveClass('bg-secondary')

    rerender(<Badge variant="destructive">Destructive</Badge>)
    expect(screen.getByText('Destructive')).toHaveClass('bg-destructive')

    rerender(<Badge variant="outline">Outline</Badge>)
    expect(screen.getByText('Outline')).toHaveClass('text-foreground')
  })

  it('applies custom className', () => {
    render(<Badge className="custom-badge">Custom</Badge>)
    
    const badge = screen.getByText('Custom')
    expect(badge).toHaveClass('custom-badge')
  })

  it('renders as span by default', () => {
    render(<Badge>Span Badge</Badge>)
    
    const badge = screen.getByText('Span Badge')
    expect(badge.tagName).toBe('SPAN')
  })

  it('renders as child component when asChild is true', () => {
    render(
      <Badge asChild>
        <a href="/test">Link Badge</a>
      </Badge>
    )
    
    const link = screen.getByRole('link', { name: /link badge/i })
    expect(link).toBeInTheDocument()
    expect(link).toHaveAttribute('href', '/test')
  })

  it('renders with children elements', () => {
    render(
      <Badge>
        <span>Icon</span>
        <span>Text</span>
      </Badge>
    )
    
    expect(screen.getByText('Icon')).toBeInTheDocument()
    expect(screen.getByText('Text')).toBeInTheDocument()
  })

  it('has correct data-slot attribute', () => {
    render(<Badge>Badge</Badge>)
    
    const badge = screen.getByText('Badge')
    expect(badge).toHaveAttribute('data-slot', 'badge')
  })
})
