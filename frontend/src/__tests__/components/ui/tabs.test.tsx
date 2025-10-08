import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs'

describe('Tabs Components', () => {
  describe('Tabs', () => {
    it('renders correctly with all components', () => {
      render(
        <Tabs defaultValue="tab1">
          <TabsList>
            <TabsTrigger value="tab1">Tab 1</TabsTrigger>
            <TabsTrigger value="tab2">Tab 2</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content 1</TabsContent>
          <TabsContent value="tab2">Content 2</TabsContent>
        </Tabs>
      )
      
      expect(screen.getByText('Tab 1')).toBeInTheDocument()
      expect(screen.getByText('Tab 2')).toBeInTheDocument()
      expect(screen.getByText('Content 1')).toBeInTheDocument()
    })

    it('shows correct content for selected tab', () => {
      render(
        <Tabs defaultValue="first">
          <TabsList>
            <TabsTrigger value="first">First</TabsTrigger>
            <TabsTrigger value="second">Second</TabsTrigger>
          </TabsList>
          <TabsContent value="first">First Content</TabsContent>
          <TabsContent value="second">Second Content</TabsContent>
        </Tabs>
      )
      
      expect(screen.getByText('First Content')).toBeInTheDocument()
      // Second content is hidden (not in document when tab is inactive)
      expect(screen.queryByText('Second Content')).not.toBeInTheDocument()
    })

    it('switches tabs when clicked', async () => {
      const user = userEvent.setup()
      
      render(
        <Tabs defaultValue="tab1">
          <TabsList>
            <TabsTrigger value="tab1">Tab One</TabsTrigger>
            <TabsTrigger value="tab2">Tab Two</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content One</TabsContent>
          <TabsContent value="tab2">Content Two</TabsContent>
        </Tabs>
      )
      
      expect(screen.getByText('Content One')).toBeInTheDocument()
      
      const tab2 = screen.getByText('Tab Two')
      await user.click(tab2)
      
      expect(screen.getByText('Content Two')).toBeInTheDocument()
      // Content One is hidden when tab switches
      expect(screen.queryByText('Content One')).not.toBeInTheDocument()
    })

    it('applies custom className to Tabs', () => {
      const { container } = render(
        <Tabs defaultValue="tab1" className="custom-tabs">
          <TabsList>
            <TabsTrigger value="tab1">Tab</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content</TabsContent>
        </Tabs>
      )
      
      const tabs = container.querySelector('[data-slot="tabs"]')
      expect(tabs).toHaveClass('custom-tabs')
    })

    it('applies custom className to TabsList', () => {
      const { container } = render(
        <Tabs defaultValue="tab1">
          <TabsList className="custom-list">
            <TabsTrigger value="tab1">Tab</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content</TabsContent>
        </Tabs>
      )
      
      const list = container.querySelector('[data-slot="tabs-list"]')
      expect(list).toHaveClass('custom-list')
    })

    it('applies custom className to TabsTrigger', () => {
      render(
        <Tabs defaultValue="tab1">
          <TabsList>
            <TabsTrigger value="tab1" className="custom-trigger">Custom Tab</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content</TabsContent>
        </Tabs>
      )
      
      const trigger = screen.getByText('Custom Tab')
      expect(trigger).toHaveClass('custom-trigger')
    })

    it('disables trigger when disabled prop is passed', () => {
      render(
        <Tabs defaultValue="tab1">
          <TabsList>
            <TabsTrigger value="tab1">Active Tab</TabsTrigger>
            <TabsTrigger value="tab2" disabled>Disabled Tab</TabsTrigger>
          </TabsList>
          <TabsContent value="tab1">Content</TabsContent>
          <TabsContent value="tab2">Disabled Content</TabsContent>
        </Tabs>
      )
      
      const disabledTab = screen.getByText('Disabled Tab')
      expect(disabledTab).toBeDisabled()
    })

    it('renders with controlled value', () => {
      render(
        <Tabs value="controlled">
          <TabsList>
            <TabsTrigger value="controlled">Controlled</TabsTrigger>
            <TabsTrigger value="other">Other</TabsTrigger>
          </TabsList>
          <TabsContent value="controlled">Controlled Content</TabsContent>
          <TabsContent value="other">Other Content</TabsContent>
        </Tabs>
      )
      
      expect(screen.getByText('Controlled Content')).toBeInTheDocument()
      // Other content is hidden when not selected
      expect(screen.queryByText('Other Content')).not.toBeInTheDocument()
    })
  })
})
