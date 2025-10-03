import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@/__tests__/utils/test-utils'
import userEvent from '@testing-library/user-event'
import { Input } from '@/components/ui/input'

describe('Input Component', () => {
  it('renders correctly', () => {
    render(<Input placeholder="Enter text" />)
    
    const input = screen.getByPlaceholderText('Enter text')
    expect(input).toBeInTheDocument()
  })

  it('handles text input', async () => {
    const user = userEvent.setup()
    render(<Input placeholder="Type here" />)
    
    const input = screen.getByPlaceholderText('Type here')
    await user.type(input, 'Hello World')
    
    expect(input).toHaveValue('Hello World')
  })

  it('can be disabled', async () => {
    const user = userEvent.setup()
    render(<Input disabled placeholder="Disabled input" />)
    
    const input = screen.getByPlaceholderText('Disabled input')
    expect(input).toBeDisabled()
    
    // Try to type - should not work
    await user.type(input, 'Test')
    expect(input).toHaveValue('')
  })

  it('supports different input types', () => {
    const { rerender } = render(<Input type="text" data-testid="input" />)
    expect(screen.getByTestId('input')).toHaveAttribute('type', 'text')

    rerender(<Input type="email" data-testid="input" />)
    expect(screen.getByTestId('input')).toHaveAttribute('type', 'email')

    rerender(<Input type="password" data-testid="input" />)
    expect(screen.getByTestId('input')).toHaveAttribute('type', 'password')

    rerender(<Input type="number" data-testid="input" />)
    expect(screen.getByTestId('input')).toHaveAttribute('type', 'number')
  })

  it('calls onChange handler', async () => {
    const user = userEvent.setup()
    const handleChange = vi.fn()
    render(<Input onChange={handleChange} placeholder="Test input" />)
    
    const input = screen.getByPlaceholderText('Test input')
    await user.type(input, 'test')
    
    // Should be called for each character typed
    expect(handleChange).toHaveBeenCalled()
    expect(handleChange).toHaveBeenCalledTimes(4) // 't', 'e', 's', 't'
  })

  it('applies custom className', () => {
    render(<Input className="custom-input-class" data-testid="input" />)
    
    const input = screen.getByTestId('input')
    expect(input).toHaveClass('custom-input-class')
  })

  it('forwards ref correctly', () => {
    const ref = { current: null as HTMLInputElement | null }
    render(<Input ref={ref} data-testid="input" />)
    
    expect(ref.current).toBeInstanceOf(HTMLInputElement)
    expect(ref.current).toBe(screen.getByTestId('input'))
  })

  it('handles required attribute', () => {
    render(<Input required data-testid="input" />)
    
    const input = screen.getByTestId('input')
    expect(input).toBeRequired()
  })

  it('handles min and max length', () => {
    render(<Input minLength={3} maxLength={10} data-testid="input" />)
    
    const input = screen.getByTestId('input')
    expect(input).toHaveAttribute('minLength', '3')
    expect(input).toHaveAttribute('maxLength', '10')
  })

  it('handles pattern attribute', () => {
    render(<Input pattern="[0-9]*" data-testid="input" />)
    
    const input = screen.getByTestId('input')
    expect(input).toHaveAttribute('pattern', '[0-9]*')
  })
})
