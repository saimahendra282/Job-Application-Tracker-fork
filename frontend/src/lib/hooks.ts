import { useState, useEffect, useRef, useCallback } from 'react';

/**
 * Custom hook for debounced search functionality
 * @param initialValue - Initial value for the search
 * @param delay - Delay in milliseconds for debouncing (default: 300ms)
 * @returns Object containing debouncedValue, value, and setValue
 */
export function useDebounce<T>(initialValue: T, delay: number = 300) {
  const [value, setValue] = useState<T>(initialValue);
  const [debouncedValue, setDebouncedValue] = useState<T>(initialValue);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return {
    value,
    debouncedValue,
    setValue,
  };
}

/**
 * Custom hook for keyboard shortcuts
 * @param keys - Array of keys to listen for (e.g., ['Control', 'k'])
 * @param callback - Function to call when keys are pressed
 */
export function useKeyboardShortcut(keys: string[], callback: () => void) {
  const callbackRef = useRef(callback);
  
  useEffect(() => {
    callbackRef.current = callback;
  });

  useEffect(() => {
    const handleKeydown = (event: KeyboardEvent) => {
      if (keys.every(key => {
        if (key === 'Control') return event.ctrlKey;
        if (key === 'Meta') return event.metaKey;
        if (key === 'Alt') return event.altKey;
        if (key === 'Shift') return event.shiftKey;
        return event.key.toLowerCase() === key.toLowerCase();
      })) {
        event.preventDefault();
        callbackRef.current();
      }
    };

    document.addEventListener('keydown', handleKeydown);
    return () => document.removeEventListener('keydown', handleKeydown);
  }, [keys]);
}

/**
 * Custom hook for local storage with type safety
 * @param key - Local storage key
 * @param initialValue - Initial value if key doesn't exist
 * @returns Tuple of [value, setValue] similar to useState
 */
export function useLocalStorage<T>(key: string, initialValue: T): [T, (value: T) => void] {
  const [storedValue, setStoredValue] = useState<T>(() => {
    try {
      if (typeof window === 'undefined') {
        return initialValue;
      }
      const item = window.localStorage.getItem(key);
      return item ? JSON.parse(item) : initialValue;
    } catch (error) {
      console.warn(`Error reading localStorage key "${key}":`, error);
      return initialValue;
    }
  });

  const setValue = useCallback((value: T) => {
    try {
      setStoredValue(value);
      if (typeof window !== 'undefined') {
        window.localStorage.setItem(key, JSON.stringify(value));
      }
    } catch (error) {
      console.warn(`Error setting localStorage key "${key}":`, error);
    }
  }, [key]);

  return [storedValue, setValue];
}