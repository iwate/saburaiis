import { Stack } from '@fluentui/react';
import { useBreadcrumbs } from '../assemblies/Breadcrumbs';

export default function App() {
  useBreadcrumbs([]);

  return (
    <Stack style={{ width: '100%' }}>
      <Stack tokens={{ childrenGap: 8, padding: 16 }} style={{ width: '100%', overflowY: 'auto' }}>
        <h2>Get latest information</h2>
        <a href="https://github.com/iwate/saburaiis" target="_blank" rel="noopener">https://github.com/iwate/saburaiis</a>
      </Stack>
    </Stack>
    
  )
}
