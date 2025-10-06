"use client";

import { Input } from '@/components/ui/input';
import { Search, X } from 'lucide-react';
import { forwardRef, useImperativeHandle, useRef } from 'react';
import { Button } from '@/components/ui/button';

interface SearchInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
}

export interface SearchInputRef {
  focus: () => void;
}

export const SearchInput = forwardRef<SearchInputRef, SearchInputProps>(
  ({ value, onChange, placeholder = "Search applications...", className }, ref) => {
    const inputRef = useRef<HTMLInputElement>(null);

    useImperativeHandle(ref, () => ({
      focus: () => {
        inputRef.current?.focus();
      },
    }));

    const handleClear = () => {
      onChange('');
      inputRef.current?.focus();
    };

    return (
      <div className={`relative flex-1 ${className}`}>
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-neutral-500" />
        <Input
          ref={inputRef}
          type="text"
          placeholder={placeholder}
          value={value}
          onChange={(e) => onChange(e.target.value)}
          className="pl-10 pr-10"
        />
        {value && (
          <Button
            type="button"
            variant="ghost"
            size="sm"
            className="absolute right-1 top-1/2 h-7 w-7 -translate-y-1/2 p-0"
            onClick={handleClear}
          >
            <X className="h-3 w-3" />
            <span className="sr-only">Clear search</span>
          </Button>
        )}
      </div>
    );
  }
);

SearchInput.displayName = 'SearchInput';