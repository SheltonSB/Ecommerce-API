import { cn } from './utils';

describe('cn utility', () => {
  it('merges class names and handles falsy values', () => {
    const result = cn('base', false && 'hidden', 'text', undefined, 'p-4');
    expect(result).toBe('base text p-4');
  });
});
