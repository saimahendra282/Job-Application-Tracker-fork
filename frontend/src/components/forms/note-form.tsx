"use client";

import { useState, useEffect, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { noteSchema, type NoteFormData } from '@/lib/validation';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import type { Note, NoteType } from '@/lib/types';
import { Bold, Italic, Underline, List } from 'lucide-react';

interface NoteFormProps {
  applicationId: number;
  note?: Note;
  onSubmit: (data: NoteFormData & { content: string }) => void;
  onCancel: () => void;
  isLoading?: boolean;
}

export function NoteForm({ applicationId, note, onSubmit, onCancel, isLoading }: NoteFormProps) {
  const editorRef = useRef<HTMLDivElement | null>(null);
  const titleInputRef = useRef<HTMLInputElement | null>(null);
  const [editorError, setEditorError] = useState('');
  
  const form = useForm<NoteFormData>({
    resolver: zodResolver(noteSchema),
    defaultValues: note ? {
      applicationId: note.applicationId,
      title: note.title,
      content: note.content,
      noteType: note.noteType,
    } : {
      applicationId,
      title: '',
      content: '',
      noteType: 'Research' as NoteType,
    },
  });

  useEffect(() => {
    if (editorRef.current && note) {
      editorRef.current.innerHTML = note.content;
    }
    // Focus on first input when form opens
    titleInputRef.current?.focus();
  }, [note]);

  // Focus on first error field
  useEffect(() => {
    const errors = form.formState.errors;
    if (errors.title) {
      titleInputRef.current?.focus();
    } else if (errors.content || editorError) {
      editorRef.current?.focus();
    }
  }, [form.formState.errors, editorError]);

  function execFormatting(command: string) {
    document.execCommand(command);
    editorRef.current?.focus();
  }

  function handleFormSubmit(data: NoteFormData) {
    const content = editorRef.current?.innerHTML?.trim() || '';
    
    if (!content) {
      const errorMsg = 'Content is required';
      form.setError('content', { message: errorMsg });
      setEditorError(errorMsg);
      editorRef.current?.focus();
      // Announce error to screen readers
      const announcement = document.createElement('div');
      announcement.setAttribute('role', 'alert');
      announcement.setAttribute('aria-live', 'assertive');
      announcement.textContent = errorMsg;
      announcement.className = 'sr-only';
      document.body.appendChild(announcement);
      setTimeout(() => document.body.removeChild(announcement), 1000);
      return;
    }

    setEditorError('');
    onSubmit({ ...data, content });
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleFormSubmit)} className="space-y-4">
        <FormField
          control={form.control}
          name="title"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Title *</FormLabel>
              <FormControl>
                <Input 
                  placeholder="Enter note title" 
                  {...field}
                  ref={titleInputRef}
                  aria-required="true"
                  aria-invalid={!!form.formState.errors.title}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="noteType"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Category *</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Select category" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value="Research">Research</SelectItem>
                  <SelectItem value="Interview">Interview</SelectItem>
                  <SelectItem value="Follow-up">Follow-up</SelectItem>
                  <SelectItem value="General">General</SelectItem>
                  <SelectItem value="Offer">Offer</SelectItem>
                </SelectContent>
              </Select>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormItem>
          <FormLabel htmlFor="note-content-editor">Content *</FormLabel>
          <div className="border rounded-md">
            <div className="flex items-center gap-1 border-b p-2 bg-neutral-50 dark:bg-neutral-900" role="toolbar" aria-label="Text formatting toolbar">
              <Button 
                type="button" 
                variant="ghost" 
                size="sm" 
                onClick={() => execFormatting('bold')} 
                aria-label="Bold"
              >
                <Bold className="h-4 w-4" />
              </Button>
              <Button 
                type="button" 
                variant="ghost" 
                size="sm" 
                onClick={() => execFormatting('italic')} 
                aria-label="Italic"
              >
                <Italic className="h-4 w-4" />
              </Button>
              <Button 
                type="button" 
                variant="ghost" 
                size="sm" 
                onClick={() => execFormatting('underline')} 
                aria-label="Underline"
              >
                <Underline className="h-4 w-4" />
              </Button>
              <Button 
                type="button" 
                variant="ghost" 
                size="sm" 
                onClick={() => execFormatting('insertUnorderedList')} 
                aria-label="Bulleted list"
              >
                <List className="h-4 w-4" />
              </Button>
            </div>
            <div
              ref={editorRef}
              contentEditable
              className="min-h-[160px] p-3 text-sm outline-none prose prose-sm max-w-none dark:prose-invert focus:ring-2 focus:ring-neutral-950 dark:focus:ring-neutral-300"
              aria-label="Note content editor"
              suppressContentEditableWarning
            />
          </div>
          <FormMessage>{form.formState.errors.content?.message}</FormMessage>
        </FormItem>

        <div className="flex justify-end gap-2">
          <Button type="button" variant="ghost" onClick={onCancel} disabled={isLoading}>
            Cancel
          </Button>
          <Button type="submit" disabled={isLoading}>
            {isLoading ? 'Saving...' : note ? 'Update Note' : 'Add Note'}
          </Button>
        </div>
      </form>
    </Form>
  );
}
