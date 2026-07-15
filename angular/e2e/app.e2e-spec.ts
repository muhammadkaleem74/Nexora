import { NexoraTemplatePage } from './app.po';

describe('Nexora App', function () {
    let page: NexoraTemplatePage;

    beforeEach(() => {
        page = new NexoraTemplatePage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('app works!');
    });
});
