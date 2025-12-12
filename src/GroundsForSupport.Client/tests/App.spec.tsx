import { expect, test } from 'vitest';
import App from '@/App.tsx';
import { render, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';

test('counter button increments the count', () => {
  const screen = render(<App />);

  const button = screen.getByRole('button');

  fireEvent.click(button);
  fireEvent.click(button);

  expect(button).toHaveTextContent('count is 2');
});
