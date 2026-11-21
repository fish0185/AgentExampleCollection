import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideCopilotKit } from "@copilotkit/angular"; 
import { HttpAgent } from '@ag-ui/client';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideCopilotKit({
      // Register as 'default' so <copilot-chat /> connects automatically
      selfManagedAgents: {
        default: new HttpAgent({
          url: 'http://localhost:5112/agui',
        }),
      },
      // Optional: render backend MAF tool calls in the chat stream
      defaultToolRendering: true,
    }),
  ]
};
