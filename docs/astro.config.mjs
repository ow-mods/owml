// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import starlightThemeNova from 'starlight-theme-nova'

const site = "https://owml.outerwildsmods.com/";

const og_image = `${site}owml_banner.webp`;

export default defineConfig({
	site,
	integrations: [
		starlight({
			title: 'Outer Wilds Mod Loader',
			plugins: [
				starlightThemeNova(),
			],
			logo: { src: './src/assets/images/owml_icon.svg' },
			editLink: {
				baseUrl: 'https://github.com/ow-mods/owml/edit/master/docs/',
      },
			head: [
				{ tag: "meta", attrs: {property: "og:image", content: og_image}, },
				{ tag: "meta", attrs: {property: "og:image:width", content: "1602"}, },
				{ tag: "meta", attrs: {property: "og:image:height", content: "694"}, },
				{ tag: "meta", attrs: {name: "twitter:card", content: "summary"}, },
				{ tag: "meta", attrs: {name: "twitter:image", content: og_image}, },
				{ tag: "meta", attrs: {name: "theme-color", content: "#ffab8a"}, },
			],
			social: [
				{ icon: 'discord', label: "Discord", href: "https://discord.gg/wusTQYbYTc", },
				{ icon: 'github', label: 'GitHub', href: 'https://github.com/ow-mods/owml' },
			  { icon: 'seti:c-sharp', label: "Nuget", href: "https://www.nuget.org/packages/OWML", },
			],
			sidebar: [
				{
					label: "Start Here",
					slug: "start-here"
				},
				{
					label: 'Guides',
					autogenerate: { directory: 'guides' },
				},
				{
					label: 'Mod Helper',
					autogenerate: { directory: 'mod-helper' },
				},
				{
					label: "Schemas",
					autogenerate: { directory: "schemas" },
				}
			],
		}),
	],
});
