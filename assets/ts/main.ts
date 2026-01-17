// 通用全局样式与功能初始化
document.addEventListener('DOMContentLoaded', () => {
  // 导航栏滚动效果
  const header = document.querySelector('header');
  window.addEventListener('scroll', () => {
    if (window.scrollY > 50) {
      header?.classList.add('scrolled');
    } else {
      header?.classList.remove('scrolled');
    }
  });

  // 移动端菜单切换
  const menuToggle = document.getElementById('menu-toggle');
  const navMenu = document.getElementById('nav-menu');
  menuToggle?.addEventListener('click', () => {
    navMenu?.classList.toggle('active');
    menuToggle.classList.toggle('active');
  });

  // 回到顶部按钮
  const backToTop = document.getElementById('back-to-top');
  window.addEventListener('scroll', () => {
    if (window.scrollY > 300) {
      backToTop?.classList.add('show');
    } else {
      backToTop?.classList.remove('show');
    }
  });
  backToTop?.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });

  // 文章内容 代码块 语言标识 + 一键复制 功能 (✅修复BUG+优化完成)
  const highlights = document.querySelectorAll(".article-content div.highlight");
  const copyText = `📄拷贝`,
    copiedText = `已拷贝!`;

  highlights.forEach((highlight) => {
    const copyButton = document.createElement("button");
    copyButton.textContent = copyText; // ✅优化：用textContent保证表情正常显示
    copyButton.classList.add("copyCodeButton");
    highlight.appendChild(copyButton);

    const codeBlock = highlight.querySelector("code[data-lang]");
    // ✅修复致命BUG：先判断是否获取到元素，再取值，彻底杜绝控制台报错
    if (!codeBlock) return;
    // 获取语言
    const lang = codeBlock.getAttribute("data-lang");

    copyButton.addEventListener("click", () => {
      navigator.clipboard
        .writeText(codeBlock.textContent)
        .then(() => {
          copyButton.textContent = copiedText;
          setTimeout(() => {
            copyButton.textContent = copyText;
          }, 1000);
        })
        .catch((err) => {
          alert(err);
          console.log("Something went wrong", err);
        });
    });

    // Add language code button
    const languageButton = document.createElement("button");
    languageButton.innerHTML = lang.toUpperCase() + "&nbsp;&nbsp;";
    languageButton.classList.add("languageCodeButton");
    highlight.appendChild(languageButton);
  });

  // 暗黑模式切换初始化
  new StackColorScheme(document.getElementById("dark-mode-toggle"));
});

// 暗黑模式核心类 - 完整保留，必不可少
class StackColorScheme {
  private toggle: HTMLElement | null;
  private scheme: 'light' | 'dark' | 'auto';

  constructor(toggleElement: HTMLElement | null) {
    this.toggle = toggleElement;
    this.scheme = this.getSavedScheme();
    this.init();
    this.bindToggle();
  }

  private getSavedScheme(): 'light' | 'dark' | 'auto' {
    const saved = localStorage.getItem('stack-color-scheme');
    return (saved as 'light' | 'dark' | 'auto') || 'auto';
  }

  private init() {
    this.applyScheme(this.scheme);
    this.syncToggleState();
  }

  private applyScheme(scheme: 'light' | 'dark' | 'auto') {
    const root = document.documentElement;
    root.classList.remove('light', 'dark');
    if (scheme === 'auto') {
      const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      root.classList.add(isDark ? 'dark' : 'light');
    } else {
      root.classList.add(scheme);
    }
  }

  private syncToggleState() {
    if (!this.toggle) return;
    this.toggle.setAttribute('data-scheme', this.scheme);
  }

  private bindToggle() {
    if (!this.toggle) return;
    this.toggle.addEventListener('click', () => {
      this.scheme = this.scheme === 'light' ? 'dark' : this.scheme === 'dark' ? 'auto' : 'light';
      localStorage.setItem('stack-color-scheme', this.scheme);
      this.applyScheme(this.scheme);
      this.syncToggleState();
    });
  }
}