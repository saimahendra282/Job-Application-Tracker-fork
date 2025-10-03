import { describe, it, expect } from 'vitest'
import { render, screen } from '@/__tests__/utils/test-utils'
import {
  Table,
  TableHeader,
  TableBody,
  TableFooter,
  TableHead,
  TableRow,
  TableCell,
  TableCaption,
} from '@/components/ui/table'

describe('Table Components', () => {
  describe('Table', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Test</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      expect(screen.getByText('Test')).toBeInTheDocument()
    })

    it('has correct wrapper with overflow', () => {
      const { container } = render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Test</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const wrapper = container.querySelector('[data-slot="table-container"]')
      expect(wrapper).toBeInTheDocument()
      expect(wrapper).toHaveClass('overflow-x-auto')
    })

    it('applies custom className', () => {
      const { container } = render(
        <Table className="custom-table">
          <TableBody>
            <TableRow>
              <TableCell>Test</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const table = container.querySelector('table')
      expect(table).toHaveClass('custom-table')
    })
  })

  describe('TableHeader', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Header</TableHead>
            </TableRow>
          </TableHeader>
        </Table>
      )
      
      expect(screen.getByText('Header')).toBeInTheDocument()
    })

    it('has correct data-slot attribute', () => {
      const { container } = render(
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Header</TableHead>
            </TableRow>
          </TableHeader>
        </Table>
      )
      
      const header = container.querySelector('thead')
      expect(header).toHaveAttribute('data-slot', 'table-header')
    })
  })

  describe('TableBody', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Body Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      expect(screen.getByText('Body Content')).toBeInTheDocument()
    })

    it('has correct data-slot attribute', () => {
      const { container } = render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const tbody = container.querySelector('tbody')
      expect(tbody).toHaveAttribute('data-slot', 'table-body')
    })
  })

  describe('TableFooter', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableFooter>
            <TableRow>
              <TableCell>Footer Content</TableCell>
            </TableRow>
          </TableFooter>
        </Table>
      )
      
      expect(screen.getByText('Footer Content')).toBeInTheDocument()
    })
  })

  describe('TableRow', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Row Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      expect(screen.getByText('Row Content')).toBeInTheDocument()
    })

    it('has hover styles', () => {
      const { container } = render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const row = container.querySelector('tr')
      expect(row).toHaveClass('hover:bg-muted/50')
    })
  })

  describe('TableHead', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Column Header</TableHead>
            </TableRow>
          </TableHeader>
        </Table>
      )
      
      expect(screen.getByText('Column Header')).toBeInTheDocument()
    })

    it('renders as th element', () => {
      const { container } = render(
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Header</TableHead>
            </TableRow>
          </TableHeader>
        </Table>
      )
      
      const th = container.querySelector('th')
      expect(th).toBeInTheDocument()
      expect(th).toHaveTextContent('Header')
    })
  })

  describe('TableCell', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Cell Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      expect(screen.getByText('Cell Content')).toBeInTheDocument()
    })

    it('renders as td element', () => {
      const { container } = render(
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const td = container.querySelector('td')
      expect(td).toBeInTheDocument()
      expect(td).toHaveTextContent('Content')
    })
  })

  describe('TableCaption', () => {
    it('renders correctly', () => {
      render(
        <Table>
          <TableCaption>Table Caption</TableCaption>
          <TableBody>
            <TableRow>
              <TableCell>Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      expect(screen.getByText('Table Caption')).toBeInTheDocument()
    })

    it('renders as caption element', () => {
      const { container } = render(
        <Table>
          <TableCaption>Caption Text</TableCaption>
          <TableBody>
            <TableRow>
              <TableCell>Content</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      )
      
      const caption = container.querySelector('caption')
      expect(caption).toBeInTheDocument()
      expect(caption).toHaveTextContent('Caption Text')
    })
  })

  describe('Full Table Composition', () => {
    it('renders a complete table with all components', () => {
      render(
        <Table>
          <TableCaption>User List</TableCaption>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Email</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow>
              <TableCell>John Doe</TableCell>
              <TableCell>john@example.com</TableCell>
            </TableRow>
            <TableRow>
              <TableCell>Jane Smith</TableCell>
              <TableCell>jane@example.com</TableCell>
            </TableRow>
          </TableBody>
          <TableFooter>
            <TableRow>
              <TableCell>Total Users: 2</TableCell>
            </TableRow>
          </TableFooter>
        </Table>
      )

      expect(screen.getByText('User List')).toBeInTheDocument()
      expect(screen.getByText('Name')).toBeInTheDocument()
      expect(screen.getByText('Email')).toBeInTheDocument()
      expect(screen.getByText('John Doe')).toBeInTheDocument()
      expect(screen.getByText('john@example.com')).toBeInTheDocument()
      expect(screen.getByText('Jane Smith')).toBeInTheDocument()
      expect(screen.getByText('jane@example.com')).toBeInTheDocument()
      expect(screen.getByText('Total Users: 2')).toBeInTheDocument()
    })
  })
})
