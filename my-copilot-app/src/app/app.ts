import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CopilotChat } from "@copilotkit/angular"; 

@Component({
  imports: [RouterOutlet, CopilotChat],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('my-copilot-app');
}
