"use client";

import { useState } from 'react';
import { SettingsForm } from '@/components/forms/settings-form';
import type { SettingsFormData } from '@/lib/validation';
import { toast } from 'sonner';
import { FormErrorBoundary } from '@/components/error-boundary';

export default function SettingsPage() {
  const [isLoading, setIsLoading] = useState(false);
  
  // Default settings - in production, fetch from API/localStorage
  const defaultSettings: SettingsFormData = {
    enableReminders: true,
    showSalaryFields: false,
    weeklySummaryEmail: true,
    browserNotifications: false,
    defaultApplicationStatus: 'Applied',
    defaultPriority: 'Medium',
    reminderDaysBefore: 1,
    theme: 'system',
  };

  const handleSaveSettings = async (data: SettingsFormData) => {
    setIsLoading(true);
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // In production, save to API/localStorage
      localStorage.setItem('appSettings', JSON.stringify(data));
      
      toast.success('Settings saved successfully');
    } catch (error) {
      console.error('Failed to save settings:', error);
      toast.error('Failed to save settings');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight">Settings</h2>
        <p className="text-neutral-500">Configure preferences for your job tracker.</p>
      </div>
      
      <FormErrorBoundary>
        <SettingsForm
          settings={defaultSettings}
          onSubmit={handleSaveSettings}
          isLoading={isLoading}
        />
      </FormErrorBoundary>
    </div>
  );
}


