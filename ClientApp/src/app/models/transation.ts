import TranslationsEnvironment from './TranslationsEnvironment';

export interface Translation {
  id: string;
  key: string;
  Branch: string;
  da: string;
  en: string;
  sv: string;
  nb: string;
  environment?: TranslationsEnvironment;
}
