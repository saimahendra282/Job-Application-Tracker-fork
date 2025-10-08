import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@/__tests__/utils/test-utils'
import { ThemeToggle } from '@/components/theme-toggle'
import { useTheme } from 'next-themes'
import userEvent from '@testing-library/user-event'

// Mock next-themes
vi.mock('next-themes', () => ({
  useTheme: vi.fn(),
}))

describe('ThemeToggle Component', () => {
  it('renders correctly', () => {
    vi.mocked(useTheme).mockReturnValue({
      theme: 'light',
      setTheme: vi.fn(),
    } as any)

    render(<ThemeToggle />)
    
    const button = screen.getByRole('button')
    expect(button).toBeInTheDocument()
  })

  it('has screen reader text', () => {
    vi.mocked(useTheme).mockReturnValue({
      theme: 'light',
      setTheme: vi.fn(),
    } as any)

    render(<ThemeToggle />)
    
    expect(screen.getByText('Toggle theme')).toBeInTheDocument()
  })

  it('calls setTheme when clicked - light to dark', async () => {
    const user = userEvent.setup()
    const setThemeMock = vi.fn()
    vi.mocked(useTheme).mockReturnValue({
      theme: 'light',
      setTheme: setThemeMock,
    } as any)

    render(<ThemeToggle />)
    
    const button = screen.getByRole('button')
    await user.click(button)
    
    expect(setThemeMock).toHaveBeenCalledWith('dark')
  })

  it('calls setTheme when clicked - dark to light', async () => {
    const user = userEvent.setup()
    const setThemeMock = vi.fn()
    vi.mocked(useTheme).mockReturnValue({
      theme: 'dark',
      setTheme: setThemeMock,
    } as any)

    render(<ThemeToggle />)
    
    const button = screen.getByRole('button')
    await user.click(button)
    
    expect(setThemeMock).toHaveBeenCalledWith('light')
  })

  it('renders as ghost variant button', () => {
    vi.mocked(useTheme).mockReturnValue({
      theme: 'light',
      setTheme: vi.fn(),
    } as any)

    render(<ThemeToggle />)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('hover:bg-accent')
  })

  it('renders as icon size button', () => {
    vi.mocked(useTheme).mockReturnValue({
      theme: 'light',
      setTheme: vi.fn(),
    } as any)

    render(<ThemeToggle />)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('size-9')
  })
})
