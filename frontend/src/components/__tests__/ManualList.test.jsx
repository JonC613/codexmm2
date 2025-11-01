import { render, screen } from '@testing-library/react';
import ManualList from '../ManualList.jsx';

describe('ManualList', () => {
  it('shows empty message when no manuals provided', () => {
    render(
      <ManualList
        manuals={[]}
        selectedId={null}
        onSelect={() => {}}
        loading={false}
        pagination={{ page: 1, pageCount: 1 }}
        onPageChange={() => {}}
      />
    );

    expect(screen.getByText(/no manuals found/i)).toBeInTheDocument();
  });
});
